using Core.Components;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HotUpdate.Core.Component
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputComponent : MonoBehaviour, IComponent
    {
        private PlayerInput _playerInput;
        
        public PlayerInput PlayerInput => _playerInput;
        
        public IEntityObject EntityObject { get; private set; }
        
        void IComponent.Init(IEntityObject entityObject)
        {
            _playerInput = entityObject.GameObject.GetComponent<PlayerInput>();
        }

        public void Destroy()
        {
            _playerInput =  null;
        }
    }
}
