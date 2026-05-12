namespace Core.Pool
{
    /// <summary>
    /// 对象类型，用于对象池销毁优先级，按照枚举顺序升序排列
    /// 同时用于标记对象子池所属类型
    /// </summary>
    internal enum EObjectType : byte
    {
        /// <summary>
        /// UI
        /// </summary>
        UI,
        
        /// <summary>
        /// 音效
        /// </summary>
        SFX,
        
        /// <summary>
        /// 特效
        /// </summary>
        VFX,
        
        /// <summary>
        /// 纯游戏对象
        /// </summary>
        GameObject,
        
        /// <summary>
        /// 对象组件
        /// </summary>
        Component,
    }
}
