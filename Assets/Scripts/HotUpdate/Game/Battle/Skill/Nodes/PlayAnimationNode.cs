using System.Collections;
using HotUpdate.Base.Component;
using HotUpdate.Game.Battle.Animation;
using HotUpdate.Game.Battle.Skill.Base;
using UnityEngine;

namespace HotUpdate.Game.Battle.Skill.Nodes
{
    /// <summary>
    /// 播放动画节点
    /// </summary>
    public class PlayAnimationNode : SkillNode
    {
        private string _layerName;
        private string _stateName;
        private float _targetEndProgress;
        
        public PlayAnimationNode(ISkill skill) : base(skill)
        {
            
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override IEnumerator Execute()
        {
            // 获取施法者的动画组件
            var animationComponent = SkillContext.Caster.GetComponent<IBattleAnimationComponent>();
            // 根据配置表设置技能对应的动画状态
            animationComponent.SetAnimationState(SkillContext.SkillInfo.f_animationType);
            // 等待动画播放到普攻状态（Attack）
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(_layerName).IsName(_stateName));
            // 等待动画播放至90%且特效已结束，确保技能流程完整
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(_layerName).normalizedTime >= _targetEndProgress && !SkillContext.VFXInfo.IsAlive);
        }

        public void SetStateAnimation(string layerName, string stateName, float targetEndProgress)
        {
            _layerName = layerName;
            _stateName = stateName;
            _targetEndProgress = targetEndProgress;
        }
    }
}
