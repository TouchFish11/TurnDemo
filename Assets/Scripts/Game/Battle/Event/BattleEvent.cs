using Game.Battle.Context;

namespace Game.Battle.Event
{
    /// <summary>
    /// ս���¼��¼����ࣨ����ս���¼��̳д��࣬Я�������ģ�
    /// </summary>
    public abstract class BattleEvent
    {
        /// <summary>
        /// ս�������ģ��洢��ǰ�غϡ���ɫ�б���ȫ�����ݣ�
        /// </summary>
        public IBattleContext Context { get; }

        protected BattleEvent(IBattleContext context)
        {
            Context = context;
        }
    }
}
