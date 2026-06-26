using UnityEngine;

namespace HotUpdate.Base.Component
{
    /// <summary>
    /// 角色控制器组件
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class CharacterControllerComponent : BaseComponent
    {
        public CharacterController CharacterController { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            CharacterController = EntityObject?.GameObject.GetComponent<CharacterController>();
        }

        protected override void OnBaseDestroy()
        {
            CharacterController = null;
        }
    }
}
