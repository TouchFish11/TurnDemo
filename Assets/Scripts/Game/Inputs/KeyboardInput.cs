using System;
using UnityEngine;

namespace Game.Inputs
{
    using Input = UnityEngine.Input;

    public class KeyboardInput : IPlayerInput
    {
        public event Action<Vector2> OnMove;
        public event Action OnJump;
        public event Action OnDash;
        public event Action OnInteract;
        public event Action<bool> OnAttack;
        public event Action<float> OnMouseX;
        public event Action<float> OnMouseY;

        public void OnUpdateInput()
        {
            OnMove?.Invoke(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
            OnMouseX?.Invoke(Input.GetAxisRaw("Mouse X"));
            OnMouseY?.Invoke(Input.GetAxisRaw("Mouse Y"));
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnJump?.Invoke();
            }

            if (Input.GetMouseButtonDown(0))
            {
                OnAttack?.Invoke(true);
            }

            if (Input.GetMouseButtonUp(0))
            {
                OnAttack?.Invoke(false);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                OnDash?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                OnInteract?.Invoke();
            }
        }
    }
}
