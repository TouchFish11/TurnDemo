using HotUpdate.Battle.Object;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Battle.Summon
{
    /// <summary>
    /// �ٻ���ӿڣ�����ս���߼���
    /// </summary>
    public interface ISummon : IBattleEntityObject
    {
        /// <summary>
        /// �ٻ��ߣ����ˣ�
        /// </summary>
        IBattleEntityObject Owner { get; }

        /// <summary>
        /// ��ʼ���ٻ���
        /// </summary>
        void Init(IBattleEntityObject owner);

        /// <summary>
        /// ʣ���ж����������ñ����壩(��ѡ)
        /// </summary>
        //int RemainingActionTimes { get; }

        /// <summary>
        /// �����ж�������API��(��ѡ)
        /// </summary>
        //void ConsumeActionTime(); 
    }
}
