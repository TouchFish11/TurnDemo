namespace Core.UI.ViewController
{
    /// <summary>
    /// MVC控制器界面状态类型
    /// </summary>
    public enum EControllerState : byte
    {
        /// <summary>
        /// 正在初始化
        /// </summary>
        Initializing,
        
        /// <summary>
        /// 界面正在初始化
        /// </summary>
        Activating,
        
        /// <summary>
        /// 界面初始化显示完成，可以执行业务逻辑
        /// </summary>
        Ready,
        
        /// <summary>
        /// 界面正在隐藏
        /// </summary>
        InActivating,
        
        /// <summary>
        /// 界面已被销毁
        /// </summary>
        Destroyed
    }
}
