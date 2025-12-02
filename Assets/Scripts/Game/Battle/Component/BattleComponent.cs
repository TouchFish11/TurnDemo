using GameLogic.BattleMoudule;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// Õ½¶·×é¼þ
    /// </summary>
    public class BattleComponent : MonoBehaviour, IBattleComponent
    {
        public bool IsDeath { get; internal set; }

        public IEntityObject EntityObject { get; private set; }

        public virtual void Init(IEntityObject entityObject)
        {
            EntityObject = entityObject as IBattleEntityObject;
        }

        public virtual void Destroy()
        {
            EntityObject = null;
        }
    }
}
