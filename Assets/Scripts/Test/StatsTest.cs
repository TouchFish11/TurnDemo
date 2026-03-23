using System.Collections.Generic;
using Core.Serialize.Json;
using Test.Config;
using Test.Equip;
using Test.SO;
using UnityEngine;

namespace Test
{
    /// <summary>
    /// 属性系统测试
    /// </summary>
    public class StatsTest : MonoBehaviour
    {
        public TextAsset e1Config;
        public TextAsset e2Config;
        
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
            var config1 = JsonManager.Instance.FromJson<WeaponConfig>(e1Config.text, settings: Core.Utility.NewtonsoftJsonUtility.SerializerSettings);
            var w1 = new Weapon(config1, null, null);
            
            // 穿戴装备
            equipmentSystem.Equip(w1);
            Debug.Log($"{w1}");
            
            Debug.Log($"穿戴装备后生命值为：{statsComponent.GetFinalValue(EStatType.Hp)}");
            Debug.Log($"穿戴装备后攻击力为：{statsComponent.GetFinalValue(EStatType.Atk)}");
            Debug.Log($"穿戴装备后防御力为：{statsComponent.GetFinalValue(EStatType.Def)}");
            
            // 创建装备
            var config2 = JsonManager.Instance.FromJson<WeaponConfig>(e2Config.text, settings: Core.Utility.NewtonsoftJsonUtility.SerializerSettings);
            var w2 = new Weapon(config2,null, null);
            
            // 穿戴装备
            equipmentSystem.Equip(w2);
            Debug.Log($"{w2}");
            
            Debug.Log($"穿戴装备后生命值为：{statsComponent.GetFinalValue(EStatType.Hp)}");
            Debug.Log($"穿戴装备后攻击力为：{statsComponent.GetFinalValue(EStatType.Atk)}");
            Debug.Log($"穿戴装备后防御力为：{statsComponent.GetFinalValue(EStatType.Def)}");
            
            // 监听属性变化
            //statsComponent.GetStat(EStatType.Atk).OnValueChanged += newHp => { UpdateHealthBar(newHp); };
        }
    }
}
