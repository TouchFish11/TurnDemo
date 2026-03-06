using System.Collections;
using Core.Pool;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Core.Battle.Command
{
    /// <summary>
    /// ս������ӿ�
    /// </summary>
    public interface ICommand : IPoolData
    {
        /// <summary>
        /// �������
        /// </summary>
        IBattleEntityObject Sender { get; }

        /// <summary>
        /// �������ȼ�
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// �Ƿ���Ч
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// ִ������
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IEnumerator Execute(IBattleContext context);

        /// <summary>
        /// 执行后处理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IEnumerator ExcutePostProcess(IBattleContext context);
    }
}
