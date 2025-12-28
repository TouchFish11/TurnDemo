namespace Framework
{
    /// <summary>
    /// 事件类型枚举
    /// </summary>
    public enum E_EventType
    {
        #region 默认事件
        /// <summary>
        /// 场景加载进度(Default)_float
        /// </summary>
        E_Scene_LoadingProgress,

        /// <summary>
        /// TCP连接完成后
        /// </summary>
        E_Net_OnPostConnect,

        /// <summary>
        /// 打开界面事件
        /// </summary>
        E_OpenView,

        /// <summary>
        /// 关闭界面事件
        /// </summary>
        E_CloseView,

        #endregion

        /// <summary>
        /// 交互
        /// </summary>
        E_OnInteract,

        /// <summary>
        /// 对话
        /// </summary>
        E_OnDialogue,

    }
}
