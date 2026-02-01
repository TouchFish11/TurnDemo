namespace Core.DataPersistence.Binary
{
    /// <summary>
    /// 加载器类型枚举
    /// 用于选择指定加载器加载配置
    /// </summary>
    public enum EConfigLoadType : byte
    {
        /// <summary>
        /// Excel配置
        /// </summary>
        Excel,

        /// <summary>
        /// 编辑器配置
        /// </summary>
        Editor,
    }
}
