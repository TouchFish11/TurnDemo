namespace Core.UI.ViewController
{
    /// <summary>
    /// MVC控制器界面状态类型
    /// </summary>
    internal enum EControllerState : byte
    {
        /// <summary>
        /// 正在初始化
        /// </summary>
        Initializing,
        
        /// <summary>
        /// 界面正在初始化，表示“这个 UI 在逻辑上处于激活状态”，此时等价于实际的activeSelf为true
        /// </summary>
        Activating,
        
        /// <summary>
        /// 界面初始化显示完成，可以执行业务逻辑，UI回调虚方法依赖该状态
        /// </summary>
        Ready,
        
        /// <summary>
        /// 界面正在隐藏，表示“这个 UI 在逻辑上处于失活状态”，但是实际的activeSelf可能不是false，不应该依赖activeSelf
        /// </summary>
        InActivating,
        
        /// <summary>
        /// 界面处于逻辑销毁，实际可能没有被立刻销毁，不应该依赖，以逻辑销毁为准
        /// </summary>
        Destroyed
    }
}
