using System;
using Core.Components;
using HotUpdate.Core.Animation;
using HotUpdate.Core.Component;
using UnityEngine;

namespace HotUpdate.Animation.Core
{
    /// <summary>
    /// 动画触发器
    /// </summary>
    [RequireComponent(typeof(AnimatorComponent))]
    [DisallowMultipleComponent]
    public class AnimationTrigger : MonoBehaviour, IAnimationTrigger
    {
        public event Action<int> OnAttack;
        
        public void OnAttackTrigger(int skillId)
        {
            var stateInfo = GetComponent<Animator>().GetCurrentAnimatorStateInfo(1);
            OnAttack?.Invoke(skillId);
        }

        public IEntityObject EntityObject { get; private set; }
        
        void IComponent.Init(IEntityObject entityObject)
        {
            
        }

        public void Destroy()
        {
            EntityObject = null;
            OnAttack = null;
        }
    }
}
