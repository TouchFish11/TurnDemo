using System;
using System.Diagnostics;
using Core.Log;

namespace Core.Time
{
    /// <summary>
    /// �����ʱ��
    /// </summary>
    public class CodeTimer : IDisposable
    {
        // ��������
        private readonly string testName;
        // ���Դ���
        private readonly uint testCount;
        // �������
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
            Logger.LogDebug(TODO, $"{testName}�����Դ�����{testCount}���ܺ�ʱ��{spendTime}ms��ƽ����ʱ��{spendTime / testCount}ms");
        }
    }
}
