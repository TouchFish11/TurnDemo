using System.Collections;
using HotUpdate.Base.Component;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Skill.Nodes
{
    /// <summary>
    /// 终结技触发时的效果，用于展示Pose或播放动画
    /// </summary>
    public abstract class UltimateTriggerNode : SkillNode
    {
        // 占位
        private bool isPose;
        
        protected UltimateTriggerNode(ISkill skill) : base(skill)
        {
            
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override IEnumerator Execute()
        {
            // 都显示立绘
            yield return battleCoordinator.ExecutePreUltimateCast(skill.SkillContext.Caster, skill.SkillContext.SkillInfo);
            if (isPose)
            {
                // 终结技动画Pose
                skill.SkillContext.Caster.GetComponent<IBattleAnimationComponent>().SetUltimatePose();
            }
            else
            {
                // 先播放动画
                
                // 然后一般都是进入一个强化状态业务
                // ...
            }
        }
    }
}
