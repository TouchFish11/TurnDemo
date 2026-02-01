using Core.Components;
using Game.Battle.Objects;
using UnityEngine;

namespace Game.Battle.Component
{
    /// <summary>
    /// ս�����
    /// </summary>
    public abstract class BattleComponent : MonoBehaviour, IBattleComponent
    {
        IEntityObject IComponent.EntityObject { get; }

        public IBattleEntityObject BattleEntity { get; private set; }

        public void Init(IEntityObject entityObject)
        {
            BattleInit(entityObject as IBattleEntityObject);
        }

        public virtual void BattleInit(IBattleEntityObject battleEntity)
        {
            BattleEntity = battleEntity;
        }

        public virtual void Destroy()
        {
            BattleEntity = null;
        }
    }
}
