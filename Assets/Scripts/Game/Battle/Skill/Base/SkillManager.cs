using Framework;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

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

        /// <summary>
        /// TODO：终结技不需要走这个方法，直接放入命令即可，然后在执行命令的时候去选择目标才对
        /// 添加技能命令到回合队列
        /// </summary>
        public void AddSkillCommand(ISkill skill)
        {
            InitSkillTarget(skill);
            // 封装技能
            SkillCommand skillCommand = ServiceLocator.Get<IFactoryManager>().GetFactory<CommandFactory>().GetSkillCommand(skill);
            // 放入指令
            ServiceLocator.Get<IBattleManager>().GetContext().GetTurnManager().InsertCommand(skillCommand);
        }


        /// <summary>
        /// 添加终结技技能命令到回合队列
        /// </summary>
        /// <param name="skill"></param>
        public void AddUltimateSkillCommand(ISkill skill)
        {
            // 封装技能
            SkillCommand skillCommand = ServiceLocator.Get<IFactoryManager>().GetFactory<CommandFactory>().GetSkillCommand(skill);
            // 放入指令
            ServiceLocator.Get<IBattleManager>().GetContext().GetTurnManager().InsertCommand(skillCommand);
        }
    }
}
