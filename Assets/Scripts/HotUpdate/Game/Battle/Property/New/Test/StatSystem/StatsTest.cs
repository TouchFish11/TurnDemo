using Core.DI;
using Core.Serialize.Json;
using HotUpdate.Game.Battle.Property.New.Test.StatSystem;
using HotUpdate.Game.Battle.Property.New.Test.StatSystem.Formulas;
using HotUpdate.Game.Battle.Property.New.Test.StatSystem.Modifiers;
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
    }
}
