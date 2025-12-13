using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Framework
{
    /// <summary>
    /// 代码计时器
    /// </summary>
    public class CodeTimer : IDisposable
    {
        // 测试名称
        private readonly string testName;
        // 测试次数
        private readonly uint testCount;
        // 秒表对象
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
            double spendTime = stopwatch.Elapsed.TotalMilliseconds;
            LogManager.Log($"{testName}，测试次数：{testCount}，总耗时：{spendTime}ms，平均耗时：{spendTime / testCount}ms");
        }
    }
}
