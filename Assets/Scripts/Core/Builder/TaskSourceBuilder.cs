using System.Threading.Tasks;

namespace Core.Builder
{
    /// <summary>
    /// ����Դ������
    /// ����ͳһ�����������Դ����
    /// </summary>
    public class TaskSourceBuilder
    {
        /// <summary>
        /// �����������Դ
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TaskCompletionSource<TResult> CreateTCS<TResult>()
        {
            return new TaskCompletionSource<TResult>();
        }
    }
}
