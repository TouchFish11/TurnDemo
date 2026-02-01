using Core.Pool;
using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Status;
using Game.Battle.Status.Data;

namespace GameHotUpdate.Battle.Status
{
    /// <summary>
    /// ״̬����
    /// </summary>
    public abstract class Status : IStatus, IPoolData
    {
        // �Ƿ���Ч
        private bool _isValid;
        // �ӳ�����
        protected StatusBonusData bonusData;

        public StatusProperty StatusProperty { get; protected set; }

        public IBattleEntityObject Sourcer { get; private set; }

        public IBattleEntityObject Owner { get; private set; }

        public StatusBonusData BonusData => bonusData;

        public bool IsValid
        {
            get => _isValid;
            set
            {
                _isValid = value;
                if (value)
                {
                    OnAdd();
                }
                else
                {
                    OnRemove();
                }
            }
        }

        public void InitStatus(IBattleEntityObject sorucer, IBattleEntityObject owner, int statusId)
        {
            StatusProperty = new StatusProperty(statusId);
            bonusData = new StatusBonusData();
            Sourcer = sorucer;
            Owner = owner;
        }

        public void ChangePine(int deltaPine)
        {
            // ��������
            StatusProperty.SetCurrentPine(StatusProperty.CurrentPine + deltaPine);
            // ����Ч��
            OnPineChanged();
        }

        public virtual void TurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            OnTurnStart(owner, context);

            // �ж�ʣ��غ����������Ƿ���Ч
            if (StatusProperty.RemainingRound <= 0 || StatusProperty.CurrentPine <= 0)
            {
                IsValid = false;
            }
        }

        public virtual void TurnEnd(IBattleEntityObject owner, IBattleContext context)
        {
            OnTurnEnd(owner, context);
        }

        /// <summary>
        /// ִ�������߼�
        /// ��IsValidΪtrueʱ��������
        /// </summary>
        protected virtual void OnAdd() { }

        /// <summary>
        /// �����仯ִ��
        /// </summary>
        protected virtual void OnPineChanged() { }

        /// <summary>
        /// ִ���Ƴ��߼�
        /// ��IsValidΪfalseʱ��������
        /// </summary>
        protected virtual void OnRemove() { }

        /// <summary>
        /// �غϿ�ʼ�߼�
        /// ����غϡ���������ͬ״̬�в�ͬ�Ľ���������Զ���
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="context"></param>
        protected abstract void OnTurnStart(IBattleEntityObject owner, IBattleContext context);

        /// <summary>
        /// �غϽ����߼�
        /// ���ڻغϽ���ʱ�����������߼������������Զ���
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="context"></param>
        protected virtual void OnTurnEnd(IBattleEntityObject owner, IBattleContext context) { }

        public void ResetData()
        {
            _isValid = false;
            StatusProperty = null;
            Sourcer = null;
            Owner = null;
        }
    }
}
