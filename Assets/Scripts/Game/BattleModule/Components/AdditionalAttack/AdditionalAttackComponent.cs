using GameLogic.BattleMoudule.Core;
using GameLogic.BattleMoudule.Event;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.BattleMoudule.AdditionalAttack
{
    /// <summary>
    /// 角色追加攻击组件（管理所有追加攻击机制）
    /// </summary>
    public class AdditionalAttackComponent : MonoBehaviour, IAdditionalAttackComponent
    {
        private IBattleEntity _owner;
        // 追加攻击列表
        private readonly List<IAdditionalAttack> _additionalAttacks = new List<IAdditionalAttack>();

        public void Init(IBattleEntity owner)
        {
            _owner = owner;
            // 加载追加攻击机制（可从配置表绑定，新增机制仅需添加实现类）
            _additionalAttacks.Add(new BreakToughnessAdditionalAttack());

            // 订阅“破盾事件”（监听破盾，触发追加攻击）
            BattleEventCenter.AddListener<ToughnessBrokenEvent>(OnToughnessBrokenHandler);
        }

        /// <summary>
        /// 事件回调：破盾后触发追加攻击
        /// </summary>
        /// <param name="evt"></param>
        private void OnToughnessBrokenHandler(ToughnessBrokenEvent toughnessBrokenEvent)
        {
            // 只处理当前组件所属角色的追加攻击（即破盾者）
            if (toughnessBrokenEvent.Breaker != _owner)
            {
                return;
            }

            // 遍历所有追加攻击，判断是否满足触发条件
            foreach (var attack in _additionalAttacks)
            {
                if (attack.CanTrigger(toughnessBrokenEvent.Context, _owner, toughnessBrokenEvent.Target))
                {
                    attack.Execute(toughnessBrokenEvent.Context, _owner, toughnessBrokenEvent.Target);
                }
            }
        }
    }
}
