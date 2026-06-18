using System;
using System.Collections;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Skill.Nodes
{
    /// <summary>
    /// 处理弹射物命中触发事件节点
    /// </summary>
    public class ProcessProjectileEventNode : SkillNode
    {
        public ProcessProjectileEventNode(ISkill skill) : base(skill)
        {
            
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override IEnumerator Execute()
        {
            yield break;
        }

        public void SetProjectileEventProcessStrategy(Action<SkillContext, HitResult> skillEvent)
        {
            SkillContext.Projectile.OnTrigger += hitResult => skillEvent?.Invoke(SkillContext, hitResult);
        }
    }
}
