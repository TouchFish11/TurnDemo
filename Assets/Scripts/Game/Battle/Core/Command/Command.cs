using System.Collections;

namespace Game.Battle
{
    /// <summary>
    /// 命令基类
    /// </summary>
    public abstract class Command : ICommand
    {
        public IBattleEntityObject Sender { get; protected set; }

        public abstract int Priority { get; protected set; }

        /// <summary>
        /// 是否有效
        /// 发送者不为null且未死亡
        /// </summary>
        public virtual bool IsValid => Sender != null && !Sender.IsDead;

        public abstract IEnumerator Excute(IBattleContext context);

        public virtual void ResetData()
        {
            Sender = null;
        }
    }
}
