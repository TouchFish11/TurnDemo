using System.Collections;
using HotUpdate.Battle.Context;
using HotUpdate.Battle.Object;

namespace HotUpdate.Battle.Command
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
