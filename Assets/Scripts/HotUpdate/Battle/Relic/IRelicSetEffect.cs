using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Battle.Relic
{
    /// <summary>
    /// ������װЧ���ӿ�
    /// </summary>
    public interface IRelicSetEffect : IBattleComponent
    {
        /// <summary>
        /// ��װ����
        /// </summary>
        IBattleEntityObject Owner { get; }

        /// <summary>
        /// ��װ����
        /// </summary>
        string SetName { get; }

        /// <summary>
        /// ���������������2���ס�4���ף�
        /// </summary>
        int RequiredCount { get; }

        /// <summary>
        /// ������װ������
        /// </summary>
        /// <param name="owner"></param>
        void SetOwner(IBattleEntityObject owner);

        /// <summary>
        /// ������װЧ��
        /// </summary>
        /// <param name="owner"></param>
        void Activate(IBattleEntityObject owner); 
    }
}
