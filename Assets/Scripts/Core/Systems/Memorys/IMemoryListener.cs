namespace Core.Systems.Memorys
{
    /// <summary>
    /// 内存监听器
    /// </summary>
    public interface IMemoryListener
    {
        /// <summary>
        /// 报告时执行
        /// </summary>
        /// <param name="memoryReportData"></param>
        void OnReport(MemoryReportData memoryReportData);
    }
}
