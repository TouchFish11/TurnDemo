using UnityEngine;

namespace HotUpdate.Game.Battle.StatSystem.Modifiers
{
    public class TestConversionModifier : ConversionModifier
    {
        private float _ratio;
        private float _limit;

        public TestConversionModifier(EModifierType modifierType) : base(modifierType)
        {
            
        }

        public void Init(StatSet sources, float ratio, float limit)
        {
            source = sources;
            _ratio = ratio;
            _limit = limit;
        }

        public void SetRatio(float ratio)
        {
            _ratio = ratio;
            InvokeOnChanged();
        }

        public void SetLimit(float limit)
        {
            _limit = limit;
            InvokeOnChanged();
        }

        protected override void OnReset()
        {
            source = null;
        }

        protected override float OnGetValue(StatEvaluationContext context)
        {
            var speed = source.Stats[EStatType.Speed].FinalValue;
            speed = Mathf.Min(speed, _limit);
            return speed * _ratio;
        }
    }
}
