using GameLogic.BattleMoudule;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// Õ½¶·×é¼þ
    /// </summary>
    public abstract class BattleComponent : MonoBehaviour, IBattleComponent
    {
        public bool IsDeath { get; internal set; }

        IEntityObject IComponent.EntityObject { get; }

        //public IEntityObject EntityObject { get; private set; }

        public IBattleEntityObject BattleEntity { get; private set; }

        //public void Init(IEntityObject entityObject) { }

        void IComponent.Init(IEntityObject entityObject)
        {

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
