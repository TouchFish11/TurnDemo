namespace GameHotUpdate.Battle.Property
{
    public interface IPropertyComponent
    {
        bool IsDeath { get; }

        /// <summary>
        /// ��������ֵ
        /// </summary>
        /// <param name="dynamicPropertyType"></param>
        /// <param name="newValue"></param>
        void SetPropertyValue(E_DynamicPropertyType dynamicPropertyType, int newValue);

        /// <summary>
        /// ��ȡ����ֵ
        /// </summary>
        /// <param name="dynamicPropertyType"></param>
        /// <returns></returns>
        int GetPropertyValue(E_DynamicPropertyType dynamicPropertyType);

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <returns></returns>
        T GetProperty<T>() where T : BattleProperty;

        /// <summary>
        /// �������Լӳ�
        /// </summary>
        /// <param name="bonusType"></param>
        /// <param name="value"></param>
        void SetPropertyBonus(E_PropertyBonusType bonusType, int value);

        /// <summary>
        /// ��ȡ���Լӳ�
        /// </summary>
        /// <param name="bonusType"></param>
        /// <returns></returns>
        int GetPropertyBonus(E_PropertyBonusType bonusType);
    }
}
