using System;
using UnityEngine;

namespace Input
{
    public interface IInputEvent
    {
        event Action<Vector2> OnMove;
        
        event Action OnJump;
        
        event Action OnDash;
        
        event Action OnInteract;
        
        event Action<bool> OnAttack;

        event Action<float> OnMouseX;
        
        event Action<float> OnMouseY;
    }
}
