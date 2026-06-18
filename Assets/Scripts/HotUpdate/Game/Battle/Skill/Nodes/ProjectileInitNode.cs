using System;
using System.Collections;
using Core.DI;
using Core.Pool;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Skill.Nodes
{
    /// <summary>
    /// 弹射物初始化节点
    /// </summary>
    public class ProjectileInitNode : SkillNode
    {
        [Inject] protected IPoolManager poolManager;
     
        private IProjectileInitStrategy _projectileInitStrategy;
        
        private Action<SkillContext> onInit;
        
        protected ProjectileInitNode(ISkill skill) : base(skill)
        {
            
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override IEnumerator Execute()
        {
            onInit?.Invoke(SkillContext);
            yield break;
        }

        public void SetProjectileInitStrategy(Action<SkillContext> init)
        {
            onInit = init;
        }
    }
}
