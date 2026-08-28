using UnityEngine;

namespace HotUpdate.Game.Battle.Property.New.Test.StatSystem.Resources
{
    public abstract class ResourceBase : IResource
    {
        public float CurrentValue { get; protected set; }
    
        public float MaxValue { get; protected set; }

        protected ResourceBase(float max, float current)
        {
            MaxValue = max;
            CurrentValue = current;
        }
    
        public virtual void Gain(float amount)
        {
            CurrentValue = Mathf.Min(MaxValue, CurrentValue + amount);
        }

        public virtual void Consume(float amount)
        {
            CurrentValue = Mathf.Max(CurrentValue - amount, 0);
        }

        public void ConsumeAll()
        {
            Consume(MaxValue);
        }
    }
}
