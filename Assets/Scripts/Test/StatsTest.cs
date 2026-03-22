using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    /// <summary>
    /// 属性系统测试
    /// </summary>
    public class StatsTest : MonoBehaviour
    {
        private void Start()
        {
            // 初始化
            var obj = new GameObject("StatsSystemTest");
            var statsComponent = obj.AddComponent<StatsComponent>();

            var statsModifier = new StatsModifier(statsComponent);
            statsComponent.Init(statsModifier);
            
            // 初始化装备系统
            var equipmentSystem = new EquipmentSystem();
            statsModifier.RegisterSource(equipmentSystem);
            
            
            // 获取属性
            Debug.Log($"穿戴装备前生命值为：{statsComponent.GetFinalValue(EStatType.Hp)}");
            Debug.Log($"穿戴装备前攻击力为：{statsComponent.GetFinalValue(EStatType.Atk)}");
            Debug.Log($"穿戴装备前防御力为：{statsComponent.GetFinalValue(EStatType.Def)}");
            
            // 创建装备
            var e1 = new Equipment
            {
                id = 1,
                name = "Equipment 1",
                bonusDatas = new List<BonusData>
                {
                    new(){ StatType = EStatType.Hp, BuildValue = 100, PercentValue = 0.5f },
                    new(){ StatType = EStatType.Atk, BuildValue = 10, PercentValue = 0.1f },
                    new(){ StatType = EStatType.Def, BuildValue = 20, PercentValue = 0.3f },
                },
            };
            
            // 穿戴装备
            equipmentSystem.Equip(e1);
            Debug.Log($"{e1}");
            
            Debug.Log($"穿戴装备后生命值为：{statsComponent.GetFinalValue(EStatType.Hp)}");
            Debug.Log($"穿戴装备后攻击力为：{statsComponent.GetFinalValue(EStatType.Atk)}");
            Debug.Log($"穿戴装备后防御力为：{statsComponent.GetFinalValue(EStatType.Def)}");
            
            // 创建装备
            var e2 = new Equipment
            {
                id = 2,
                name = "Equipment 2",
                bonusDatas = new List<BonusData>
                {
                    new(){ StatType = EStatType.Atk, BuildValue = 20, PercentValue = 0.5f },
                    new(){ StatType = EStatType.Hp, BuildValue = 30, PercentValue = 0.1f },
                    new(){ StatType = EStatType.Def, BuildValue = 50, PercentValue = 0.1f },
                },
            };
            
            // 穿戴装备
            equipmentSystem.Equip(e2);
            Debug.Log($"{e2}");
            
            Debug.Log($"穿戴装备后生命值为：{statsComponent.GetFinalValue(EStatType.Hp)}");
            Debug.Log($"穿戴装备后攻击力为：{statsComponent.GetFinalValue(EStatType.Atk)}");
            Debug.Log($"穿戴装备后防御力为：{statsComponent.GetFinalValue(EStatType.Def)}");
            
            // 监听属性变化
            //statsComponent.GetStat(EStatType.Atk).OnValueChanged += newHp => { UpdateHealthBar(newHp); };
        }
    }
}
