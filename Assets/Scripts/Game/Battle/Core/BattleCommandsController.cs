using Game.Battle;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 战斗指令控制器
/// </summary>
public class BattleCommandsController
{
    // 技能命令队列
    private readonly List<ISkill> _skillCommands = new List<ISkill>();
    // 战斗上下文
    private readonly IBattleContext _context;
    // 当前执行的命令
    private ISkill _skill; 

    public int Count => _skillCommands.Count;

    public BattleCommandsController(IBattleContext context)
    {
        this._context = context;
    }

    public IEnumerator ExcuteCommand()
    {
        // 存在命令，执行
        while (_skill != null || _skillCommands.Count > 0)
        {
            GetFirst();
            // 执行技能命令
            yield return _skill.Cast(_context);
            _skill = null;

            // 检查战斗是否结束
            if (_context.GetTurnManager().CheckBattleOver())
            {
                yield break;
            }
        }
    }

    /// <summary>
    /// 获取首个命令
    /// </summary>
    /// <returns></returns>
    public void GetFirst()
    {
        if (_skillCommands.Count > 0)
        {
            _skill = _skillCommands[0];
            RemoveFirst();
        }
    }

    /// <summary>
    /// 插入指令
    /// </summary>
    /// <param name="skill"></param>
    public void InsertCommand(ISkill skill)
    {
        if (_skill == null)
        {
            _skill = skill;
            return;
        }
        else
        {
            _skillCommands.Add(skill);
            // 按优先级排序命令
            SortCommand();
            // 触发指令排队事件
            _context.GetEventBus().TriggerEvent(new CommandWaitEvent(_context, _skillCommands));
        }
    }

    /// <summary>
    /// 移除首个命令
    /// </summary>
    public void RemoveFirst()
    {
        _skillCommands.RemoveAt(0);
        // 触发指令排队事件
        _context.GetEventBus().TriggerEvent(new CommandWaitEvent(_context, _skillCommands));
    }

    /// <summary>
    /// 排序命令
    /// 按优先级排序
    /// </summary>
    private void SortCommand()
    {
        _skillCommands.Sort((c1, c2) =>
        {
            if (c1.SkillInfo.f_priority > c2.SkillInfo.f_priority)
            {
                return -1;
            }
            else if (c1.SkillInfo.f_priority < c2.SkillInfo.f_priority)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        });
    }
}
