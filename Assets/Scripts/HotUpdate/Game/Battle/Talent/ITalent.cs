using HotUpdate.Game.Battle.Event;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Talent
{
    /// <summary>
    /// �츳�ӿڣ����н�ɫ�츳ͳһʵ�֣����ô����߼���
    /// </summary>
    public interface ITalent
    {
        string Name { get; }

        IBattleEntityObject Owner { get; }

        /// <summary>
        /// ���������������¼���
        /// </summary>
        /// <param name="battleEvent"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        bool CanTrigger(BattleEvent battleEvent, IBattleEntityObject owner);

        /// <summary>
        /// �츳Ч��ִ��
        /// </summary>
        /// <param name="battleEvent"></param>
        /// <param name="owner"></param>
        void Execute(BattleEvent battleEvent, IBattleEntityObject owner);

        /// <summary>
        /// �غϿ�ʼʱ����
        /// </summary>
        //void OnTurnStartHandler(TurnStartEvent turnStartEvent);

        /// <summary>
        /// �غϽ���ʱ����
        /// </summary>
        //void OnTurnEndHandler(TurnEndEvent turnEndEvent);
    }
}
