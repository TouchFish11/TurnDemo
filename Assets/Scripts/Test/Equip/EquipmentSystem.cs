using System;
using System.Collections.Generic;

namespace Test.Equip
{
    /// <summary>
    /// 装备系统
    /// </summary>
    public class EquipmentSystem : IStatsModifierSource
    {
        private readonly List<IEquipment> _equipments = new();
        
        public event Action OnModifiersChanged;
        
        public void Equip(Equipment eq)
        {
            eq.Equip(null);
            _equipments.Add(eq);
            OnModifiersChanged?.Invoke(); // 通知变化
        }
    
        public void Unequip(Equipment eq) 
        {
            eq.UnEquip(null);
            _equipments.Remove(eq);
            OnModifiersChanged?.Invoke(); // 通知变化
        }
        
        public void GetModifier(Dictionary<EStatType, BonusData> bonusDatas)
        {
            foreach (var equipment in _equipments)
            {
                foreach (var equipmentBonusData in equipment.Config.bonusDatas)
                {
                    if (bonusDatas.TryGetValue(equipmentBonusData.StatType, out var bonusData))
                    {
                        bonusData.BuildValue += equipmentBonusData.BuildValue;
                        bonusData.PercentValue += equipmentBonusData.PercentValue;
                    }
                    else
                    {
                        bonusDatas.Add(equipmentBonusData.StatType, new BonusData
                        {
                            StatType =  equipmentBonusData.StatType,
                            BuildValue = equipmentBonusData.BuildValue,
                            PercentValue = equipmentBonusData.PercentValue
                        });
                    }
                }
            }
        }
    }
}
