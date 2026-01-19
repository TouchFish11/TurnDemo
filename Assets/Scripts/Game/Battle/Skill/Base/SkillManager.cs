using Framework;
using System.Collections.Generic;

namespace Game.Battle
{
    /// <summary>
    /// 技能管理器
    /// </summary>
    public class SkillManager : SingletonBase<SkillManager>, ISkillManager
    {
        private SkillManager()
        {

        }

        public void InitSkillTarget(ISkill skill)
        {
            // 通过目标选择管理器获取技能主目标
            IBattleEntityObject mainTaget = ServiceLocator.Get<ITargetSelectManager>().GetMainTarget();
            // 通过目标选择管理器获取技能所有目标
            List<IBattleEntityObject> selectedTargets = ServiceLocator.Get<ITargetSelectManager>().GetTargets();
            // 初始化技能
            skill.Init(mainTaget, selectedTargets);
        }


        public void AddSkillCommand(ISkill skill)
        {
            // 封装技能
            SkillCommand skillCommand = ServiceLocator.Get<IFactoryManager>().GetFactory<CommandFactory>().GetSkillCommand(skill);
            // 放入指令
            ServiceLocator.Get<IBattleManager>().GetContext().GetTurnManager().InsertCommand(skillCommand);
        }
    }
}
