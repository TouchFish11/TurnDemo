using Game;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
            // 指令排队，更新UI显示
            BattleUIScheduler.Instance.UpdateWaitingCommmand(GetRoleIcon());
        }
    }

    /// <summary>
    /// 移除首个命令
    /// </summary>
    public void RemoveFirst()
    {
        _skillCommands.RemoveAt(0);
        // 指令排队，更新UI显示
        BattleUIScheduler.Instance.UpdateWaitingCommmand(GetRoleIcon());
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


    public List<string> GetRoleIcon()
    {
        List<string> strs = new List<string>(_skillCommands.Count);

        foreach (ISkill skill in _skillCommands)
        {
            string icon = string.Empty;
            if (skill.Caster is PlayerObject playerObject)
            {
                icon = playerObject.RoleInfo.f_name;
            }
            else if(skill.Caster is MonsterObject monsterObject)
            {
                icon = monsterObject.MonsterInfo.f_name;
            }

            strs.Add(icon);
        }

        return strs;
    }
}
