using System.Collections;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Skill.Nodes
{
    /// <summary>
    /// 更新相机节点
    /// </summary>
    public class UpdateCameraNode : SkillNode
    {
        // 需要一个策略，不同的技能相机位置不一样
        private IUpdateCameraStrategy _updateCameraStrategy;
        
        public UpdateCameraNode(ISkill skill) : base(skill)
        {
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override IEnumerator Execute()
        {
            // 更新战斗相机视角
            yield return _updateCameraStrategy?.UpdateCamera(SkillContext);
        }

        public void SetUpdateCameraStrategy(IUpdateCameraStrategy strategy)
        {
            _updateCameraStrategy = strategy;
        }
    }
}
