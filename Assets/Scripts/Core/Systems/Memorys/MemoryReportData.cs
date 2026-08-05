namespace Core.Systems.Memorys
{
    /// <summary>
    /// 内存报告数据
    /// </summary>
    public readonly struct MemoryReportData
    {
        public readonly EMemoryOccupationLevel level;
        
        public MemoryReportData(EMemoryOccupationLevel level)
        {
            this.level = level;
        }
    }
}
