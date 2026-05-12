namespace Core.AssetBundles.Management
{
    /// <summary>
    /// 池化对象ID池
    /// </summary>
    public static class ObjectIdPool
    {
        /// <summary>
        /// 池化对象全局ID
        /// </summary>
        private static int _globalId;

        /// <summary>
        /// 获取池化对象全局ID，不复用
        /// </summary>
        public static int GetGlobalId()
        {
            return ++_globalId;
        }
    }
}
