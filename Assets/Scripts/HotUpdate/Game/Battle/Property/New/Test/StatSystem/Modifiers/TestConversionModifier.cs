using UnityEngine;

namespace HotUpdate.Game.Battle.Property.New.Test.StatSystem.Modifiers
{
    public class TestConversionModifier : ConversionModifier
    {
        private readonly float _ratio;
        private readonly float _limit;
        
        public TestConversionModifier(StatSet sources, float ratio, float limit) : base(sources)
        {
            _ratio = ratio;
            _limit = limit;
        }

        protected override float OnGetValue(StatEvaluationContext context)
        {
            var speed = source.Stats[EStatType.Speed].FinalValue;
            speed = Mathf.Min(speed, _limit);
            return speed * _ratio;
        }
    }
}
