using GameHotUpdate.Battle.Object;
using GameHotUpdate.Battle.Property;

namespace GameHotUpdate.Battle.Toughness.CalcStrategy
{
    /// <summary>
    /// ���Լ�����Խӿ�
    /// </summary>
    public interface IToughnessCalcStrategy
    {
        /// <summary>
        /// �������ȼ�
        /// ��ֵԽ��Խ��ִ��
        /// ��������ToughnessStrategyAttribute�е�Ҫһ��
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// ��������ֵ
        /// </summary>
        /// <param name="reducer"></param>
        /// <param name="target"></param>
        /// <param name="propertyType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        int CalcReduceToughness(IBattleEntityObject reducer, IBattleEntityObject target, E_ElementType propertyType, int value);
    }
}
