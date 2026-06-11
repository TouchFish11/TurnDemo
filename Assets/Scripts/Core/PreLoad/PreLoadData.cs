namespace Core.PreLoad
{
    /// <summary>
    /// 预加载数据
    /// </summary>
    public readonly struct PreLoadData
    {
        /// <summary>
        /// 资源名称
        /// </summary>
        public string AssetName { get; }

        public PreLoadData(string assetName)
        {
            this.AssetName = assetName;
        }
    }
}
