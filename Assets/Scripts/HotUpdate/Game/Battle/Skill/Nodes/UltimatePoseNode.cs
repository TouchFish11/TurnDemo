using System.Collections;
using Core.DI;
using Core.Pool;
using Core.Utility;
using HotUpdate.Base.Component;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Skill.Nodes
{
    /// <summary>
    /// 终结技Pose节点，播放pose动作和特效
    /// </summary>
    public class UltimatePoseNode : SkillNode
    {
        [Inject] protected IVFXManager vfxManager;
        [Inject] protected IPoolManager poolManager;
        
        private string _vfxName;
        
        public UltimatePoseNode(ISkill skill) : base(skill)
        {
        
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override IEnumerator Execute()
        {
            var projectileData = new ProjectileData(SkillContext.Caster, SkillContext.MainTarget, SkillContext.AllTargets, this);
            var projectileTrans = new ProjectileTrans(SkillContext.Caster.GameObject.transform.position, Quaternion.identity);
            var vFXInfo = poolManager.GetData<VFXInfo>();
            // 终结技动画Pose
            skill.SkillContext.Caster.GetComponent<IBattleAnimationComponent>().SetUltimatePose();
            // 终结技Pose特效
            var task = vfxManager.CreateVFX(AssetKeys.VFX_Priest_UltimatePose, projectileTrans, projectileData, vFXInfo);
            yield return TaskUtility.WaitForTask(task);
        }

        public void SetPoseVFXName(string vfxName)
        {
            _vfxName = vfxName;
        }
    }
}
