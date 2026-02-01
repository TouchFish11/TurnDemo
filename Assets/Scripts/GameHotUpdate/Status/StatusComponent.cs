using System.Collections.Generic;
using Core.Components;
using Game.Battle.Component;
using Game.Battle.Event;
using Game.Battle.Objects;
using Game.Battle.Status;
using Game.Battle.Status.Data;
using Game.Battle.Status.Enum;
using GameHotUpdate.Battle.Event.Turn;
using GameHotUpdate.Battle.Event.UI;

namespace GameHotUpdate.Status
{
    /// <summary>
    /// ״̬�������
    /// ��ɫ��״̬�����������������״̬
    /// </summary>
    [ComponentId(typeof(StatusComponent))]
    public class StatusComponent : BattleComponent, IStatusComponent
    {
        // ״̬�б�
        private readonly List<IStatus> _statuses = new List<IStatus>();
        // ״̬�ܼӳ�����
        private StatusTotalBonusData statusTotalBonus;

        public override void BattleInit(IBattleEntityObject battleEntity)
        {
            base.BattleInit(battleEntity);

            statusTotalBonus = new StatusTotalBonusData();
            // ���ĻغϿ�ʼ�¼�
            battleEntity.Context.GetEventBus().AddListener<TurnStartEvent>(OnTurnStart);
            // ���ĻغϽ����¼�
            battleEntity.Context.GetEventBus().AddListener<TurnEndEvent>(OnTurnEnd);
        }

        /// <summary>
        /// �غϿ�ʼ�¼��ص�
        /// </summary>
        /// <param name="turnStartEvent"></param>
        private void OnTurnStart(TurnStartEvent turnStartEvent)
        {
            OnTurnChanged(turnStartEvent);
        }

        /// <summary>
        /// �غϽ����¼��ص�
        /// </summary>
        /// <param name="turnEndEvent"></param>
        private void OnTurnEnd(TurnEndEvent turnEndEvent)
        {
            OnTurnChanged(turnEndEvent);
        }

        /// <summary>
        /// �غϱ仯�߼�
        /// </summary>
        /// <param name="battleEvent"></param>
        private void OnTurnChanged(BattleEvent battleEvent)
        {
            if (battleEvent.Context.GetCurrentEntity() != BattleEntity)
            {
                return;
            }

            if (battleEvent is TurnStartEvent turnStartEvent)
            {
                // ����������Ч״̬��������TurnStart
                foreach (IStatus status in _statuses)
                {
                    if (status.IsValid)
                    {
                        status.TurnStart(BattleEntity, turnStartEvent.Context);
                    }
                }
            }
            else if(battleEvent is TurnEndEvent turnEndEvent)
            {
                // ����������Ч״̬��������TurnEnd
                foreach (IStatus status in _statuses)
                {
                    if (status.IsValid)
                    {
                        status.TurnEnd(BattleEntity, turnEndEvent.Context);
                    }
                }
            }

            // �Ƴ�ʧЧ״̬
            _statuses.RemoveAll(s => !s.IsValid);
            // ����״̬�ӳ�
            UpdateStatusBonus();
            // ���½�ɫUI״̬��
            BattleEntity.Context.GetEventBus().TriggerEvent(new TurnStartStatusChangedEvent(BattleEntity.Context, BattleEntity));
        }

        /// <summary>
        /// ����״̬
        /// </summary>
        /// <param name="status"></param>
        public void AddStatus(IStatus status)
        {
            // ������ͻ
            switch ((E_ConflictType)status.StatusProperty.StatusInfo.f_conflictType)
            {
                case E_ConflictType.Add:
                    OnConflict_Add(status);
                    break;
                case E_ConflictType.Lonely:
                    OnConflict_Lonel(status);
                    break;
                case E_ConflictType.Cover:
                    OnConflict_Cover(status);
                    break;
            }

            // ����״̬�ӳ�
            UpdateStatusBonus();
            // �����¼�������״̬�����ı�UI
            BattleEntity.Context.GetEventBus().TriggerEvent(new StatusAddedEvent(BattleEntity.Context, status));
        }

        /// <summary>
        /// �Ƴ�״̬
        /// </summary>
        /// <param name="removalStrategy"></param>
        public void RemoveStatus(IStatusRemovalStrategy removalStrategy)
        {
            removalStrategy.HandleRemove(_statuses);
        }

        /// <summary>
        /// ����״̬�ӳ�
        /// </summary>
        private void UpdateStatusBonus()
        {
            // TODO�����Ż�������Ҫÿ�ζ����㣬���Ǳ仯��ʱ���ڼ���
            statusTotalBonus.UpdateTotalAtkBonus(_statuses);
            statusTotalBonus.UpdateTotalDefBonus(_statuses);
            statusTotalBonus.UpdateTotalHpBonus(_statuses);
        }

        /// <summary>
        /// �������ʹ���
        /// </summary>
        /// <param name="newStatus"></param>
        private void OnConflict_Add(IStatus newStatus)
        {
            // ���ж��Ƿ���ڸ�ID��״̬
            if (this.TryGetStatus(newStatus.StatusProperty.StatusInfo.f_id, _statuses, out IStatus status))
            {
                // ���Ӳ�������ʱд�����������õ�λ���Ӳ���
                status.ChangePine(1);
            }
            else
            {
                newStatus.IsValid = true;
                _statuses.Add(newStatus);
            }
        }

        /// <summary>
        /// �������ʹ���
        /// </summary>
        /// <param name="newStatus"></param>
        private void OnConflict_Lonel(IStatus newStatus)
        {
            newStatus.IsValid = true;
            _statuses.Add(newStatus);
        }

        /// <summary>
        /// �������ʹ���
        /// </summary>
        /// <param name="newStatus"></param>
        private void OnConflict_Cover(IStatus newStatus)
        {
            // ���ж��Ƿ���ڸ�ID��״̬
            if (this.TryGetStatus(newStatus.StatusProperty.StatusInfo.f_id, _statuses, out IStatus status))
            {
                status.IsValid = false;
                _statuses.Remove(status);
                newStatus.IsValid = true;
                _statuses.Add(newStatus);
            }
        }
    }
}
