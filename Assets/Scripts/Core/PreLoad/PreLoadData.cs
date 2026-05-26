namespace Core.PreLoad
{
    /// <summary>
    /// 预加载数据
    /// </summary>
    public readonly struct PreLoadData
    {
        public string AssetName { get; }

        public PreLoadData(string assetName)
        {
            this.AssetName = assetName;
        }
    }
}
