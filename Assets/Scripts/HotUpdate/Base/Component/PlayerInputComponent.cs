using UnityEngine;
using UnityEngine.InputSystem;

namespace HotUpdate.Base.Component
{
    /// <summary>
    /// 玩家输入组件
    /// 对原生PlayerInput的封装
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputComponent : BaseComponent
    {
        public PlayerInput PlayerInput { get; private set; }

        protected override void OnInit()
        {
            PlayerInput = EntityObject?.GameObject.GetComponent<PlayerInput>();
        }

        protected override void OnBaseDestroy()
        {
            PlayerInput =  null;
        }
    }
}
