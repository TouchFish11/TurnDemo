using System;
using System.Collections.Generic;
using Core.DI;
using HotUpdate.Game.Battle.Skill.Nodes;

namespace HotUpdate.Game.Battle.Skill.Base
{
    /// <summary>
    /// 技能节点构建管线
    /// </summary>
    public class SkillNodeBuildPipeline
    {
        private ISkill _skill;
        private readonly List<ISkillNode> _effects = new();

        public void SetSkill(ISkill skill)
        {
            _skill = skill;
        }

        public SkillNodeBuildPipeline AddMonsterPreNode()
        {
            _effects.Add(DIContainer.Create<MonsterPreNode>(parameterValues: _skill));
            return this;
        }
        
        public SkillNodeBuildPipeline AddTargetSelectNode()
        {
            _effects.Add(DIContainer.Create<TargetSelectNode>(parameterValues: _skill));
            return this;
        }
                
        public SkillNodeBuildPipeline AddProjectileInitNode(Action<SkillContext> init)
        {
            var projectileInitNode = DIContainer.Create<ProjectileInitNode>(parameterValues: _skill);
            projectileInitNode.SetProjectileInitStrategy(init);
            _effects.Add(projectileInitNode);
            return this;
        }
        
        public SkillNodeBuildPipeline AddUpdateCameraNode(IUpdateCameraStrategy strategy)
        {
            var updateCameraNode = DIContainer.Create<UpdateCameraNode>(parameterValues: _skill);
            updateCameraNode.SetUpdateCameraStrategy(strategy);
            _effects.Add(updateCameraNode);
            return this;
        }
        
        public SkillNodeBuildPipeline AddDelayNode(float delay)
        {
            var delayNode = DIContainer.Create<DelayNode>(parameterValues: _skill);
            delayNode.SetDelay(delay);
            _effects.Add(delayNode);
            return this;
        }
        
        /// <summary>
        /// 添加技能动画节点
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="stateName"></param>
        /// <param name="targetEndProgress"></param>
        /// <returns></returns>
        public SkillNodeBuildPipeline AddPlayAnimationNode(string layerName, string stateName, float targetEndProgress)
        {
            var playAnimationNode = DIContainer.Create<PlayAnimationNode>(parameterValues: _skill);
            playAnimationNode.SetStateAnimation(layerName, stateName, targetEndProgress);
            _effects.Add(playAnimationNode);
            return this;
        }
        
        public SkillNodeBuildPipeline AddCreateProjectileNode(string vfxName)
        {
            var createProjectileNode = DIContainer.Create<CreateProjectileNode>(parameterValues: _skill);
            createProjectileNode.SetVFXName(vfxName);
            _effects.Add(createProjectileNode);
            return this;
        }
        
        public SkillNodeBuildPipeline AddProcessProjectileEventNode(Action<SkillContext, HitResult> skillEvent)
        {
            var processProjectileEventNode = DIContainer.Create<ProcessProjectileEventNode>(parameterValues: _skill);
            processProjectileEventNode.SetProjectileEventProcessStrategy(skillEvent);
            _effects.Add(processProjectileEventNode);
            return this;
        }

        public SkillNodeBuildPipeline AddSkillPointCastNode()
        {
            var skillPointCastNode = DIContainer.Create<SkillPointCastNode>(parameterValues: _skill);
            _effects.Add(skillPointCastNode);
            return this;
        }
        
        public SkillNodeBuildPipeline AddUltimatePoseNode(string poseVfxName)
        {
            var ultimatePoseNode = DIContainer.Create<UltimatePoseNode>(parameterValues: _skill);
            ultimatePoseNode.SetPoseVFXName(poseVfxName);
            _effects.Add(ultimatePoseNode);
            return this;
        }
        
        public SkillNodeBuildPipeline AddUltimateWaitTriggerNode()
        {
            var ultimateWaitTriggerNode = DIContainer.Create<UltimateWaitTriggerNode>(parameterValues: _skill);
            _effects.Add(ultimateWaitTriggerNode);
            return this;
        }
        
        public SkillNodeBuildPipeline AddUltimateDisplayIllustrationNode()
        {
            var ultimateWaitTriggerNode = DIContainer.Create<UltimateDisplayIllustrationNode>(parameterValues: _skill);
            _effects.Add(ultimateWaitTriggerNode);
            return this;
        }
        
        public SkillNodeBuildPipeline AddUltimateFlowNode(IUltimateFlowStrategy ultimateFlowStrategy)
        {
            var ultimateFlowNode = DIContainer.Create<UltimateFlowNode>(parameterValues: _skill);
            ultimateFlowNode.SetUltimateFlowStrategy(ultimateFlowStrategy);
            _effects.Add(ultimateFlowNode);
            return this;
        }
        
        public List<ISkillNode> Build()
        {
            var list = new List<ISkillNode>(_effects);
            _effects.Clear();
            return list;
        }
    }
}
