using Core.Components;
using UnityEngine;

namespace HotUpdate.Base.Component
{
    /// <summary>
    /// 角色控制器组件
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class CharacterControllerComponent : MonoBehaviour, IComponent
    {
        private CharacterController _controller;
        
        public IEntityObject EntityObject { get; private set; }
        
        public CharacterController CharacterController => _controller;
        
        void IComponent.Init(IEntityObject entityObject)
        {
            _controller = entityObject.GameObject.GetComponent<CharacterController>();
        }

        public void Destroy()
        {
            EntityObject = null;
            _controller = null;
        }
    }
}
