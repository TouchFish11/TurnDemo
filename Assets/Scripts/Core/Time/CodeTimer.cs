using System;
using System.Diagnostics;
using Core.Log;

namespace Core.Time
{
    /// <summary>
    /// 代码计时器
    /// </summary>
    public class CodeTimer : IDisposable
    {
        // 测试名称
        private readonly string testName;
        // 测试数量
        private readonly uint testCount;
        // 计时对象
        private readonly Stopwatch stopwatch;

        public CodeTimer(string testName, uint testCount)
        {
            this.testName = testName;
            this.testCount = testCount;
            stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            stopwatch.Stop();
            var spendTime = stopwatch.Elapsed.TotalMilliseconds;
            Logger.LogDebug(ELogTags.Time, $"测试名称'{testName}'；测试数量'{testCount}'；总时间'{spendTime}'ms；平均时间'{spendTime / testCount}'ms");
        }
    }
}
