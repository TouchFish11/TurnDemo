using System.Collections;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Command
{
    /// <summary>
    /// 战斗指令基类
    /// </summary>
    public abstract class Command : ICommand
    {
        public IBattleEntityObject Sender { get; protected set; }

        public abstract int Priority { get; protected set; }

        /// <summary>
        /// 指令是否有效
        /// 若发送方为null且发送方死亡则无效
        /// </summary>
        public virtual bool IsValid => Sender != null && !Sender.IsDead;

        public abstract IEnumerator Execute(IBattleContext context);

        /// <summary>
        /// 执行后处理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract IEnumerator ExcutePostProcess(IBattleContext context);

        public virtual void ResetData()
        {
            Sender = null;
        }
    }
}
