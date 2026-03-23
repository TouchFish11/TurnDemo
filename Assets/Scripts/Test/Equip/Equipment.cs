using System;
using System.Text;
using Core.Components;
using Test.Config;
using Test.Equip.Effect;
using UnityEngine;

namespace Test.Equip
{
    /// <summary>
    /// 装备
    /// </summary>
    public abstract class Equipment : IEquipment
    {
        private readonly IEquipEffect _equipEffect;
        private readonly ITriggerCondition  _triggerCondition;
        private bool _isActive;
        
        public EquipmentConfig Config { get; }
        
        protected Equipment(EquipmentConfig config, ITriggerCondition condition, IEquipEffect equipEffect)
        {
            Config = config;
            _triggerCondition = condition;
            _equipEffect = equipEffect;
        }
        
        public void Equip(IEntityObject entityObject)
        {
            OnEquip(entityObject);
            // 立即检查一次
            OnConditionChanged(entityObject);
        }

        public void UnEquip(IEntityObject entityObject)
        {
            OnUnEquip(entityObject);
        }
        
        /// <summary>
        /// 在装备时，订阅可触发装备效果的相关事件
        /// </summary>
        /// <param name="entityObject"></param>
        protected abstract void OnEquip(IEntityObject entityObject);
        
        /// <summary>
        /// 在卸下装备时，移除订阅的事件
        /// </summary>
        /// <param name="entityObject"></param>
        protected abstract void OnUnEquip(IEntityObject entityObject);

        /// <summary>
        /// 在角色状态变化时执行
        /// </summary>
        /// <param name="entityObject"></param>
        private void OnConditionChanged(IEntityObject entityObject)
        {
            if (_triggerCondition == null || _equipEffect == null || entityObject == null)
            {
                Debug.Log($"{Config.name}装备，触发条件为null/装备效果为null/角色为null");
                return;
            }
            
            if (_isActive && !_triggerCondition.CanSatisfy(entityObject))
            {
                _equipEffect.Remove(entityObject);
                _equipEffect.IsVaild = false;
                _isActive = false;
            }
            else if (!_isActive && _triggerCondition.CanSatisfy(entityObject))
            {
                _equipEffect.Apply(entityObject);
                _equipEffect.IsVaild = true;
                _isActive = true;
            }
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var bonusData in Config.bonusDatas)
            {
                switch (bonusData.StatType)
                {
                    case EStatType.Hp:
                        sb.Append($"{(bonusData.BuildValue != 0 ? $"生命 +{bonusData.BuildValue}" : "")}，");
                        sb.Append($"{(bonusData.PercentValue != 0 ? $"生命 +{bonusData.PercentValue * 100}%" : "")}；");
                        break;
                    case EStatType.Atk:
                        sb.Append($"{(bonusData.BuildValue != 0 ? $"攻击 +{bonusData.BuildValue}" : "")}，");
                        sb.Append($"{(bonusData.PercentValue != 0 ? $"攻击 +{bonusData.PercentValue * 100}%" : "")}；");
                        break;
                    case EStatType.Def:
                        sb.Append($"{(bonusData.BuildValue != 0 ? $"防御 +{bonusData.BuildValue}" : "")}，");
                        sb.Append($"{(bonusData.PercentValue != 0 ? $"防御 +{bonusData.PercentValue * 100}%" : "")}；");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return $"已穿戴:{Config.id}，名称:{Config.name}，加成信息:{sb}";
        }
    }
}
