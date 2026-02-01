using System.Collections;
using Game.Battle.Context;
using Game.Battle.Objects;

namespace Game.Battle.Command
{
    /// <summary>
    /// �������
    /// </summary>
    public abstract class Command : ICommand
    {
        public IBattleEntityObject Sender { get; protected set; }

        public abstract int Priority { get; protected set; }

        /// <summary>
        /// �Ƿ���Ч
        /// �����߲�Ϊnull��δ����
        /// </summary>
        public virtual bool IsValid => Sender != null && !Sender.IsDead;

        public abstract IEnumerator Execute(IBattleContext context);

        public virtual void ResetData()
        {
            Sender = null;
        }
    }
}
