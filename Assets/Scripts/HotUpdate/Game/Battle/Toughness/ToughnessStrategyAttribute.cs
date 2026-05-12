using System;

namespace HotUpdate.Game.Battle.Toughness
{
    /// <summary>
    /// ������ز���ö��
    /// </summary>
    public enum E_ToughnessStrategyType
    {
        /// <summary>
        /// �ܷ������ж�����
        /// </summary>
        ReduceJudge,

        /// <summary>
        /// �������������
        /// </summary>
        ValueCalculate
    }

    /// <summary>
    /// ���Բ�������
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ToughnessStrategyAttribute : Attribute
    {
        /// <summary>
        /// ��������
        /// �����ж��������ֵ����
        /// </summary>
        public E_ToughnessStrategyType StrategyType { get; }

        /// <summary>
        /// �������ȼ�
        /// ��ֵԽ��Խ��ִ��
        /// </summary>
        public int Priority { get; }

        public ToughnessStrategyAttribute(E_ToughnessStrategyType strategyType, int priority)
        {
            StrategyType = strategyType;
            Priority = priority;
        }
    }
}