using System.Collections.Generic;

namespace HotUpdate.Battle.Relic
{
    /// <summary>
    /// �����ӿڣ���װЧ��+����Ч�����룩
    /// </summary>
    public interface IRelic
    {
        int RelicID {  get; }

        string Name { get; }

        /// <summary>
        /// ϡ�ж�
        /// </summary>
        E_RelicRarity Rarity { get; }

        /// <summary>
        /// ����Ч�����繥��+10%,��������
        /// </summary>
        List<RelicEffect> SingleEffects { get; } 
    }
}
