using System.Collections.Generic;
using Core.Log;
using Game.Battle.Component;
using Game.Battle.Context;
using Game.Battle.Enum;
using Game.Battle.Objects;
using Game.Battle.Property;
using Game.Property;
using GameHotUpdate.Battle.Event.General;

namespace GameHotUpdate.Property
{
    /// <summary>
    /// �������
    /// ����ս��ʵ��ĸ�������
    /// </summary>
    public abstract class PropertyComponent : BattleComponent, IPropertyComponent
    {
        // �������͵��ӳ���ֵ���ٷֱȣ�ӳ��
        private readonly Dictionary<E_PropertyBonusType, int> _bonusToValueMap = new();
        // ս������
        protected BattleProperty battleProperty;
        // ս��������
        protected IBattleContext battleContext;

        public bool IsDeath { get; protected set; }

        public override void BattleInit(IBattleEntityObject battleEntity)
        {
            base.BattleInit(battleEntity);
            battleContext = battleEntity.Context;
        }

        /// <summary>
        /// ��������ֵ
        /// </summary>
        /// <param name="dynamicPropertyType"></param>
        /// <param name="newValue"></param>
        public virtual void SetPropertyValue(E_DynamicPropertyType dynamicPropertyType, int newValue)
        {
            switch (dynamicPropertyType)
            {
                case E_DynamicPropertyType.CurrentHp:
                    int currentHpDelta = battleProperty.CurrentHp - newValue;
                    battleProperty.CurrentHp = newValue;
                    battleContext.GetEventBus().TriggerEvent(new HpChangedEvent(battleContext, newValue, battleProperty.MaxHp, currentHpDelta, BattleEntity));
                    break;
                case E_DynamicPropertyType.MaxHp:
                    int maxHpDelta = battleProperty.MaxHp - newValue;
                    battleProperty.MaxHp = newValue;
                    battleContext.GetEventBus().TriggerEvent(new HpChangedEvent(battleContext, battleProperty.CurrentHp, newValue, maxHpDelta, BattleEntity));
                    break;
                case E_DynamicPropertyType.TotalAtk:
                    battleProperty.TotalAtk = newValue;

                    break;
                case E_DynamicPropertyType.TotalDef:
                    battleProperty.TotalDef = newValue;

                    break;
                case E_DynamicPropertyType.CurrentSpeed:
                    battleProperty.CurrentSpeed = newValue;

                    break;
                case E_DynamicPropertyType.TotalCrit:
                    battleProperty.TotalCrit = newValue;
                    break;
                case E_DynamicPropertyType.TotalCritDmg:
                    battleProperty.TotalCritDmg = newValue;
                    break;
                case E_DynamicPropertyType.CurrentShield:
                    int currentShieldDelta = battleProperty.CurrentShield - newValue;
                    battleProperty.CurrentShield = newValue;
                    battleContext.GetEventBus().TriggerEvent(new ShieldChangedEvent(battleContext, battleProperty.CurrentShield, BattleEntity, currentShieldDelta));
                    break;
            }
        }

        /// <summary>
        /// ��ȡ����ֵ
        /// </summary>
        /// <param name="dynamicPropertyType"></param>
        /// <returns></returns>
        public virtual int GetPropertyValue(E_DynamicPropertyType dynamicPropertyType)
        {
            switch (dynamicPropertyType)
            {
                case E_DynamicPropertyType.CurrentHp:
                    return battleProperty.CurrentHp;
                case E_DynamicPropertyType.MaxHp:
                    return battleProperty.MaxHp;
                case E_DynamicPropertyType.TotalAtk:
                    return battleProperty.TotalAtk;
                case E_DynamicPropertyType.TotalDef:
                    return battleProperty.TotalDef;
                case E_DynamicPropertyType.CurrentSpeed:
                    return battleProperty.CurrentSpeed;
                case E_DynamicPropertyType.TotalCrit:
                    return battleProperty.TotalCrit;
                case E_DynamicPropertyType.TotalCritDmg:
                    return battleProperty.TotalCritDmg;
                default:
                    LogManager.LogError($"δ�ҵ���̬�������ͣ�{dynamicPropertyType}���ѷ���Ĭ��ֵ{default}");
                    return 0;
            }
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <returns></returns>
        public T GetProperty<T>() where T : BattleProperty
        {
            return battleProperty as T;
        }

        /// <summary>
        /// �������Լӳ�
        /// </summary>
        /// <param name="bonusType"></param>
        /// <param name="value"></param>
        public void SetPropertyBonus(E_PropertyBonusType bonusType, int value)
        {
            if (!_bonusToValueMap.TryAdd(bonusType, value))
            {
                _bonusToValueMap[bonusType] += value;
            }
        }

        /// <summary>
        /// ��ȡ���Լӳ�
        /// </summary>
        /// <param name="bonusType"></param>
        /// <returns></returns>
        public int GetPropertyBonus(E_PropertyBonusType bonusType)
        {
            if (_bonusToValueMap.TryGetValue(bonusType, out var value))
            {
                return value;
            }

            LogManager.LogWarning($"�����Լӳɲ����ڣ�{bonusType}���ѷ���{default}");
            return 0;
        }
    }
}
