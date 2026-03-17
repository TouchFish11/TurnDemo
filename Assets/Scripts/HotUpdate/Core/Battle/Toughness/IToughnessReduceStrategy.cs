using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.Battle.Property;

namespace HotUpdate.Core.Battle.Toughness
{
    /// <summary>
    /// ������������
    /// </summary>
    public interface IToughnessReduceStrategy
    {
        /// <summary>
        /// �������ȼ�
        /// ��ֵԽ��Խ��ִ��
        /// ��������ToughnessStrategyAttribute�е�Ҫһ��
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// �ܷ���������
        /// </summary>
        /// <param name="reducer"></param>
        /// <param name="target"></param>
        /// <param name="propertyType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool CanReduceToughness(IBattleEntityObject reducer, IBattleEntityObject target, E_ElementType propertyType, int value);
    }
}
