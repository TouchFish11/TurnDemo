using HotUpdate.Base.ECModule;
using UnityEngine;

namespace HotUpdate.Game.Interact
{
    /// <summary>
    /// 交互触发器
    /// </summary>
    [ComponentId]
    [RequireComponent(typeof(Collider))]
    public class InteractTrigger : BaseComponent
    {
        private IInteractable _interactable;
        private BoxCollider _collider;
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="interactable"></param>
        public void Init(IInteractable interactable)
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
            _collider.center = new Vector3(0, -0.5f, 0);
            _collider.size = new Vector3(4, 4, 4);
            
            _interactable = interactable;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            var interactComponent = other.GetComponent<IEntityObject>().GetComponent<InteractComponent>();
            if (interactComponent)
            {
                interactComponent.AddInteract(_interactable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var interactComponent = other.GetComponent<IEntityObject>().GetComponent<InteractComponent>();
            if (interactComponent)
            {
                interactComponent.RemoveInteract(_interactable);
            }
        }

        protected override void OnBaseDestroy()
        {
            _interactable = null;
            _collider =  null;
            base.OnBaseDestroy();
        }
    }
}
