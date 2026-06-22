using HotUpdate.Base.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HotUpdate.Base.Component
{
    /// <summary>
    /// 玩家输入组件
    /// 对原生PlayerInput的封装
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputComponent : MonoBehaviour, IComponent
    {
        public PlayerInput PlayerInput { get; private set; }

        public IEntityObject EntityObject { get; private set; }
        
        void IComponent.Init(IEntityObject entityObject)
        {
            PlayerInput = entityObject.GameObject.GetComponent<PlayerInput>();
        }

        public void Destroy()
        {
            PlayerInput =  null;
            EntityObject = null;
        }
    }
}
