using Core.Log;
using HotUpdate.Core.Battle.Event;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Battle.Talent.Talents
{
    /// <summary>
    /// ϣ�������֡�����ɱ���˺��ö����ж��غϣ�
    /// </summary>
    public class ReappearanceTalent : ITalent
    {
        public string Name { get; } = "����";

        public IBattleEntityObject Owner { get; }

        // ���غ��Ƿ��Ѵ�������ֹ��δ�����
        //private bool _hasTriggeredThisTurn = false; 

        public ReappearanceTalent(IBattleEntityObject owner)
        {
            Owner = owner;
        }

        public bool CanTrigger(BattleEvent battleEvent, IBattleEntityObject owner)
        {
            // if (battleEvent is not TurnEndEvent turnEndEvt)
            // {
            //     return false;
            // }
            return true;
            // ����������1. �ǽ�ɫ�����ж������¼� 2. ���غϻ�ɱ����(����жϵ������Լ����ܵ�) 3. δ������
            //return turnEndEvt.CurrentBattleEntity == owner && turnEndEvt.HasKilledEnemy && !_hasTriggeredThisTurn;
        }

        public void Execute(BattleEvent battleEvent, IBattleEntityObject owner)
        {
            //var turnEndEvt = (TurnEndEvent)battleEvent;
            LogManager.Log($"\n���츳������{owner.GameObject.name}�����츳��{Name}����");
            LogManager.Log($"{owner.GameObject.name}��ö����ж��غϣ�");

            // �����߼����޸��ж����У�����ɫ������ף����ú��Ĳ�API������ֱ�Ӳ�����(�޸�:Ӧ���ǻ�ö���غ�,�����ǲ������)
            // turnEndEvt.Context.GetTurnManager().InsertToActionHead(owner);
            //_hasTriggeredThisTurn = true; // ��Ǳ��غ��Ѵ���
        }

        // public void OnTurnStartHandler(TurnStartEvent turnStartEvent)
        // {
        //     if (turnStartEvent.CurrentBattleEntity == Owner)
        //     {
        //         //_hasTriggeredThisTurn = false;
        //     }
        // }

        // public void OnTurnEndHandler(TurnEndEvent turnEndEvent)
        // {
        //     /* �غϽ���ʱ����Ҫ���� */
        // }
    }
}
