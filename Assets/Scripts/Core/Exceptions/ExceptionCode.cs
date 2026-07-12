namespace Core.Exceptions
{
    /// <summary>
    /// 异常码
    /// </summary>
    public static class ExceptionCode
    {
        /// <summary>
        /// AB包加载错误码
        /// </summary>
        public const int AssetBundleLoadErrorCode = 1001;
        
        /// <summary>
        /// 资源加载错误码
        /// </summary>
        public const int AssetLoadErrorCode = 1002;
        
        /// <summary>
        /// 资源批量加载错误码
        /// </summary>
        public const int AssetsLoadErrorCode = 1003;
        
        /// <summary>
        /// AB包卸载错误码
        /// </summary>
        public const int AssetBundleUnloadErrorCode = 1004;
        
        /// <summary>
        /// 无效的资源句柄访问错误码
        /// </summary>
        public const int InvalidHandleAccessErrorCode = 1005;
        
        /// <summary>
        /// 网络错误码
        /// </summary>
        public const int NetworkErrorCode = 1006;
        
        /// <summary>
        /// UI初始化错误
        /// </summary>
        public const int UIInitializeErrorCode = 1007;
    }
}
