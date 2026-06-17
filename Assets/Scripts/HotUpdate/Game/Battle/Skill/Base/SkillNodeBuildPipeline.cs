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
                
        public SkillNodeBuildPipeline AddProjectileInitNode(IProjectileInitStrategy strategy)
        {
            var projectileInitNode = DIContainer.Create<ProjectileInitNode>(parameterValues: _skill);
            projectileInitNode.SetProjectileInitStrategy(strategy);
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
        
        public SkillNodeBuildPipeline AddNode<T>() where T : class, ISkillNode
        {
            _effects.Add(DIContainer.Create<T>(parameterValues: _skill));
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
