using HotUpdate.Base.Object;
using UnityEngine;

namespace HotUpdate.Base.Component
{
    /// <summary>
    /// 动画控制器组件
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimatorComponent : MonoBehaviour, IComponent
    {
        private Animator _animator;
    
        public IEntityObject EntityObject { get; private set; }
        
        public Animator Animator => _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        void IComponent.Init(IEntityObject entityObject)
        {

        }

        public void Destroy()
        {
            EntityObject = null;
            _animator = null;
        }
    }
}
