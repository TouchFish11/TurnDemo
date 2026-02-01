using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Status.Data;

namespace Game.Battle.Status
{
    /// <summary>
    /// ״̬�ӿ�
    /// </summary>
    public interface IStatus
    {
        /// <summary>
        /// ״̬�Ƿ���Ч
        /// </summary>
        bool IsValid { get; set; }

        /// <summary>
        /// ��Դ
        /// </summary>
        IBattleEntityObject Sourcer { get; }

        /// <summary>
        /// ӵ��
        /// </summary>
        IBattleEntityObject Owner { get; }

        /// <summary>
        /// ״̬����
        /// </summary>
        StatusProperty StatusProperty { get; }

        /// <summary>
        /// ״̬�ӳ�����
        /// </summary>
        StatusBonusData BonusData { get; }

        /// <summary>
        /// �غϿ�ʼʱ����Ч�߼�
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="context"></param>
        void TurnStart(IBattleEntityObject owner, IBattleContext context);

        /// <summary>
        /// �غϽ���ʱ����Ч�߼�
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="context"></param>
        void TurnEnd(IBattleEntityObject owner, IBattleContext context);

        /// <summary>
        /// ��ʼ��״̬
        /// </summary>
        /// <param name="sorucer"></param>
        /// <param name="owner"></param>
        /// <param name="statusId"></param>
        void InitStatus(IBattleEntityObject sorucer, IBattleEntityObject owner, int statusId);

        /// <summary>
        /// �ı����
        /// �����ⲿ�޸�״̬����
        /// </summary>
        /// <param name="deltaPine"></param>
        void ChangePine(int deltaPine);
    }
}
