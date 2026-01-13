using Game;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

/// <summary>
/// 战斗指令控制器
/// </summary>
public class BattleCommandsController
{
    // 战斗命令列表
    private readonly List<ICommand> _battleCommands = new List<ICommand>();
    // 战斗上下文
    private readonly IBattleContext _context;
    // 当前执行的命令
    private ICommand _command; 

    public int Count => _battleCommands.Count;

    public BattleCommandsController(IBattleContext context)
    {
        this._context = context;
    }

    public IEnumerator ExcuteCommand()
    {
        // 存在命令，执行
        while (_command != null || _battleCommands.Count > 0)
        {
            GetFirst();
            // 执行技能命令
            yield return _command.Excute(_context);
            _command = null;

            // 执行完命令后，移除死亡的实体
            _context.GetTurnManager().RemoveDeadMonster();

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
        if (_battleCommands.Count > 0)
        {
            _command = _battleCommands[0];
            RemoveFirst();
        }
    }

    /// <summary>
    /// 插入指令
    /// </summary>
    /// <param name="command"></param>
    public void InsertCommand(ICommand command)
    {
        if (_command == null)
        {
            _command = command;
            return;
        }
        else
        {
            _battleCommands.Add(command);
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
        _battleCommands.RemoveAt(0);
        // 指令排队，更新UI显示
        BattleUIScheduler.Instance.UpdateWaitingCommmand(GetRoleIcon());
    }

    /// <summary>
    /// 排序命令
    /// 按优先级排序
    /// </summary>
    private void SortCommand()
    {
        _battleCommands.Sort((c1, c2) =>
        {
            if (c1.Priority > c2.Priority)
            {
                return -1;
            }
            else if (c1.Priority < c2.Priority)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        });
    }

    // 暂时这样处理
    public List<string> GetRoleIcon()
    {
        List<string> strs = new List<string>(_battleCommands.Count);
        foreach (ICommand command in _battleCommands)
        {
            string icon = string.Empty;

            // 技能命令获取角色/怪物图标
            if (command is SkillCommand skillCommand)
            {
                if (skillCommand.Skill.Caster is PlayerObject playerObject)
                {
                    icon = playerObject.RoleInfo.f_name;
                }
                else if (skillCommand.Skill.Caster is MonsterObject monsterObject)
                {
                    icon = monsterObject.MonsterInfo.f_name;
                }
            }
            // 其它命令获取其它图标即可
            else
            {

            }
            strs.Add(icon);
        }

        return strs;
    }
}
