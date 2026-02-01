using Core.Components;
using Game.Interact;
using UnityEngine;

namespace GameHotUpdate.Interact
{
    /// <summary>
    /// ����������
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class InteractTrigger : MonoBehaviour
    {
        // ��ȡ��ǰ����Ľ����߼���ʵ��IInteractable�������
        private IInteractable interactable;
        private BoxCollider _collider;
        
        private void Awake()
        {
            _collider = this.GetComponent<BoxCollider>();
            _collider.isTrigger = true;
            _collider.center = new Vector3(0, -0.5f, 0);
            _collider.size = new Vector3(4, 4, 4);
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
    }
}
