using Core.Components;
using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Object;
using UnityEngine;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 战斗组件
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
