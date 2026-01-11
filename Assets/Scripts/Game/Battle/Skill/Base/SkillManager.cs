using System.Collections.Generic;
using Framework;
using Game.Battle;

/// <summary>
/// 技能管理器
/// </summary>
public class SkillManager : SingletonBase<SkillManager>, ISkillManager
{
    private SkillManager()
    {

    }

    /// <summary>
    /// 添加技能命令到回合队列
    /// </summary>
    public void AddSkillCommand(ISkill skill, IBattleEntityObject entityObject)
    {
        // 通过目标选择管理器获取技能主目标
        IBattleEntityObject mainTaget = ServiceLocator.Get<ITargetSelectManager>().GetMainTarget();
        // 通过目标选择管理器获取技能所有目标
        List<IBattleEntityObject> selectedTargets = ServiceLocator.Get<ITargetSelectManager>().GetTargets();
        // 初始化技能
        skill.Init(mainTaget, selectedTargets);
        // 放入指令
        ServiceLocator.Get<IBattleManager>().GetContext().GetTurnManager().InsertCommand(skill);
    }
}
