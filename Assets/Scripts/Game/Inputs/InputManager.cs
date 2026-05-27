using System;
using UnityEngine;

namespace Game.Inputs
{
    public class InputManager : MonoBehaviour, IInputEvent
    {
        public static InputManager Instance { get; private set; }
        private IPlayerInput _activeInput;

        public event Action<Vector2> OnMove
        {
            add => _activeInput.OnMove += value;
            remove => _activeInput.OnMove -= value;
        }
        
        public event Action OnJump
        {
            add => _activeInput.OnJump += value;
            remove => _activeInput.OnJump -= value;
        }
        
        public event Action OnDash
        {
            add => _activeInput.OnDash += value;
            remove => _activeInput.OnDash -= value;
        }
        
        public event Action OnInteract
        {
            add => _activeInput.OnInteract += value;
            remove => _activeInput.OnInteract -= value;
        }
        
        public event Action<bool> OnAttack
        {
            add => _activeInput.OnAttack += value;
            remove => _activeInput.OnAttack -= value;
        }

        public event Action<float> OnMouseX
        {
            add => _activeInput.OnMouseX += value;
            remove => _activeInput.OnMouseX -= value;
        }
        
        public event Action<float> OnMouseY
        {
            add => _activeInput.OnMouseY += value;
            remove => _activeInput.OnMouseY -= value;
        }

        private void Awake()
        {
            Instance = this;
            SwitchInput(new KeyboardInput());
        }

        public void SwitchInput(IPlayerInput activeInput)
        {
            _activeInput = activeInput;
        }

        private void Update()
        {
            _activeInput?.OnUpdateInput();
        }
    }
}
