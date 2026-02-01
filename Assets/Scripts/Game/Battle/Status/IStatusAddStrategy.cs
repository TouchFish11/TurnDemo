using System.Collections.Generic;
using Game.Battle.Objects;

namespace Game.Battle.Status
{
    /// <summary>
    /// ״̬���Ӳ���
    /// ��װ��ͬ���ܶ�Ӧ��ͬ״̬�������߼�
    /// </summary>
    public interface IStatusAddStrategy
    {
        /// <summary>
        /// ����״̬
        /// </summary>
        /// <param name="sourcer"></param>
        /// <param name="targets"></param>
        /// <param name="statusIds"></param>
        void ToAdd(IBattleEntityObject sourcer, List<IBattleEntityObject> targets, params int[] statusIds);
    }
}
