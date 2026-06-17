using System.Collections;
using Core.DI;
using Core.Utility;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.VFX;

namespace HotUpdate.Game.Battle.Skill.Nodes
{
    /// <summary>
    /// 创建弹射物节点
    /// </summary>
    public class CreateProjectileNode : SkillNode
    {
        [Inject] private IVFXManager _vfxManager;

        private string _vfxName;
        
        public CreateProjectileNode(ISkill skill) : base(skill)
        {
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override IEnumerator Execute()
        {
            // 创建普攻特效：从资源配置中获取普攻特效资源并生成
            var task = _vfxManager.CreateVFX(_vfxName, SkillContext.ProjectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo);
            yield return TaskUtility.WaitForTask(task, projectile => SkillContext.Projectile = projectile);
        }

        public void SetVFXName(string vfxName)
        {
            _vfxName = vfxName;
        }
    }
}
