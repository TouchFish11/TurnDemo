using System;
using System.Collections.Generic;
using Core.DI;
using Core.Serialize.Json;
using HotUpdate.Game.Battle.Property.New.Test.Equip;
using HotUpdate.Game.Battle.Property.New.Test.StatSystem;
using HotUpdate.Game.Battle.Property.New.Test.StatSystem.Formulas;
using HotUpdate.Game.Battle.Property.New.Test.StatSystem.Modifiers;
using Test.Config;
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

        private void Awake()
        {
            var speedBase = 100;
            var stat = new Stat(EStatType.Speed, new BaseFormula(), new LinearFormula());
            stat.AddModifier(new TestConversionModifier(new StatSet(), 0.007f, 100));
            Debug.Log($"{stat.FinalValue}");
        }

        private void Start()
        {
            // 初始化
            var obj = new GameObject("StatsSystemTest");
            var statsComponent = obj.AddComponent<StatsComponent>();
            
            // 初始化装备系统
            var equipmentSystem = new EquipmentSystem();
            
            // 获取属性
            Debug.Log($"穿戴装备前生命值为：{statsComponent.GetFinalValue(EStatType.Hp)}");
            Debug.Log($"穿戴装备前攻击力为：{statsComponent.GetFinalValue(EStatType.Atk)}");
            Debug.Log($"穿戴装备前防御力为：{statsComponent.GetFinalValue(EStatType.Def)}");
            
            // 创建装备
            var config1 = DIContainer.Create<JsonManager>().FromJson<WeaponConfig>(e1Config.text, settings: NewtonsoftJsonUtility.DefaultSerializerSettings);
            var w1 = new Weapon(config1, null, null);
            
            // 穿戴装备
            equipmentSystem.Equip(w1);
            Debug.Log($"{w1}");
            
            Debug.Log($"穿戴装备后生命值为：{statsComponent.GetFinalValue(EStatType.Hp)}");
            Debug.Log($"穿戴装备后攻击力为：{statsComponent.GetFinalValue(EStatType.Atk)}");
            Debug.Log($"穿戴装备后防御力为：{statsComponent.GetFinalValue(EStatType.Def)}");
            
            // 创建装备
            var config2 = DIContainer.Create<JsonManager>().FromJson<WeaponConfig>(e2Config.text, settings: NewtonsoftJsonUtility.DefaultSerializerSettings);
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
