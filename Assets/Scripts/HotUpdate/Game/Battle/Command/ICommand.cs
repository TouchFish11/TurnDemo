using System.Collections;
using Core.Pool;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Command
{
    /// <summary>
    /// 指令接口
    /// </summary>
    public interface ICommand : IPoolData
    {
        /// <summary>
        /// 指令发送方
        /// </summary>
        IBattleEntityObject Sender { get; }

        /// <summary>
        /// 指令优先级，越小越先执行
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// 指令是否有效
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// ִ指令执行逻辑
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
