using System;
using Core.Components;

namespace HotUpdate.Core.Animation
{
    public interface IAnimationTrigger : IComponent
    {
        event Action<int> OnAttack;
        void OnAttackTrigger(int skillId);
    }
}
