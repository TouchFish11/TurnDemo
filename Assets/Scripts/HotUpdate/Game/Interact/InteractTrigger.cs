using HotUpdate.Base.Component;
using HotUpdate.Base.Object;
using UnityEngine;

namespace HotUpdate.Game.Interact
{
    /// <summary>
    /// 交互触发器
    /// </summary>
    [ComponentId(typeof(InteractTrigger))]
    [RequireComponent(typeof(Collider))]
    public class InteractTrigger : MonoBehaviour, IComponent
    {
        private IInteractable interactable;
        private BoxCollider _collider;
        
        public IEntityObject EntityObject { get; private set; }
        
        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
            _collider.center = new Vector3(0, -0.5f, 0);
            _collider.size = new Vector3(4, 4, 4);
        }
        
        void IComponent.Init(IEntityObject entityObject, IComponentCore<IComponent> componentCore)
        {
            EntityObject = entityObject;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="interactable"></param>
        public void Init(IInteractable interactable)
        {
            this.interactable = interactable;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            var interactComponent = other.GetComponent<IEntityObject>().GetComponent<InteractComponent>();
            if (interactComponent)
            {
                interactComponent.AddInteract(interactable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var interactComponent = other.GetComponent<IEntityObject>().GetComponent<InteractComponent>();
            if (interactComponent)
            {
                interactComponent.RemoveInteract(interactable);
            }
        }
        
        public void Destroy()
        {
            interactable = null;
            EntityObject = null;
            _collider =  null;
        }
    }
}
