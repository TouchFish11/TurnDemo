using Framework;
using GameLogic.BattleMoudule;
using GameLogic.BattleMoudule.AdditionalAttack;
using GameLogic.BattleMoudule.Core;
using GameLogic.BattleMoudule.Relic;
using UnityEngine;

namespace GameLogic.BattleMoudule.Entity
{
    /// <summary>
    /// 战斗角色组件
    /// ——管理角色的属性
    /// </summary>
    public class BattleCharacterComponent : MonoBehaviour, IBattleCharacterComponent
    {
        private IBattleEntity _owner;

        public bool IsDeath { get; internal set; }

        public void Init(IBattleEntity owner)
        {
            _owner = owner;
        }
    }
}
