using System;
using HotUpdate.Base.Component;
using UnityEngine;

namespace HotUpdate.Game.Animation.Core
{
    /// <summary>
    /// 动画触发器
    /// </summary>
    [RequireComponent(typeof(AnimatorComponent))]
    [DisallowMultipleComponent]
    public class AnimationTrigger : MonoBehaviour
    {
        public event Action<int> OnAttack;
        
        public void OnAttackTrigger(int skillId)
        {
            var stateInfo = GetComponent<Animator>().GetCurrentAnimatorStateInfo(1);
            OnAttack?.Invoke(skillId);
        }
        
        private void OnDestroy()
        {
            OnAttack = null;
        }
    }
}
