using System.Collections.Generic;
using HotUpdate.Battle.Property;

namespace HotUpdate.Battle.Toughness
{
    /// <summary>
    /// ����״̬
    /// </summary>
    public class ToughnessState
    {
        /// <summary>
        /// ��������
        /// </summary>
        public List<E_ElementType> WeakPropertys { get; private set; }

        /// <summary>
        /// ��ǰ����ֵ
        /// </summary>
        public int CurrentToughnessValue { get; private set; }

        /// <summary>
        /// �������ֵ
        /// </summary>
        public int MaxToughnessVaue { get; private set; }

        /// <summary>
        /// �Ƿ��ѻ���
        /// </summary>
        public bool IsBroken => CurrentToughnessValue <= 0;

        public ToughnessState(List<E_ElementType> weakPropertys, int initialValue)
        {
            WeakPropertys = weakPropertys;
            CurrentToughnessValue = MaxToughnessVaue = initialValue;
        }

        /// <summary>
        /// ��������ֵ
        /// </summary>
        /// <param name="current"></param>
        /// <param name="max"></param>
        public void SetToughnessValue(int current, int max)
        {
            CurrentToughnessValue = current;
            MaxToughnessVaue = max;
        }
    }
}
