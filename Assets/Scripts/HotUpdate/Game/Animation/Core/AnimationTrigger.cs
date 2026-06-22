using System;
using HotUpdate.Base.Component;
using HotUpdate.Base.Object;
using UnityEngine;

namespace HotUpdate.Game.Animation.Core
{
    /// <summary>
    /// 动画触发器
    /// </summary>
    [RequireComponent(typeof(AnimatorComponent))]
    [DisallowMultipleComponent]
    public class AnimationTrigger : MonoBehaviour, IComponent
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

        public IEntityObject EntityObject { get; }
        
        public void Init(IEntityObject entityObject)
        {

        }

        public void Destroy()
        {

        }
    }
}
