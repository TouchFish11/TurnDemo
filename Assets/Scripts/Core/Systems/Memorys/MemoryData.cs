namespace Core.Systems.Memorys
{
    /// <summary>
    /// 内存数据
    /// </summary>
    public readonly struct MemoryData
    {
        public readonly EMemoryOccupationLevel level;
        
        public MemoryData(EMemoryOccupationLevel level)
        {
            this.level = level;
        }
    }
}
