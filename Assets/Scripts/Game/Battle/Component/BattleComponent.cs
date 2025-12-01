using Framework;
using GameLogic.BattleMoudule;
using GameLogic.BattleMoudule.AdditionalAttack;
using GameLogic.BattleMoudule.Core;
using GameLogic.BattleMoudule.Relic;
using UnityEngine;

namespace GameLogic.BattleMoudule.Entity
{
    /// <summary>
    /// 战斗组件
    /// ——管理角色的战斗属性
    /// </summary>
    public class BattleComponent : MonoBehaviour, IBattleComponent
    {
        public bool IsDeath { get; internal set; }

        public IEntityObject EntityObject { get; private set; }

        public void Init(IEntityObject entityObject)
        {
            EntityObject = entityObject as IBattleEntityObject;
        }
    }
}
