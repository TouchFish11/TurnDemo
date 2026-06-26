using UnityEngine;

namespace HotUpdate.Base.Component
{
    /// <summary>
    /// 动画控制器组件
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimatorComponent : BaseComponent
    {
        public Animator Animator { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Animator = GetComponent<Animator>();
        }

        protected override void OnBaseDestroy()
        {
            Animator = null;
        }
    }
}
