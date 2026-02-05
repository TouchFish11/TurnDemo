using System.Collections;
using Core.Reflection;
using Core.Service;
using Game.Battle.Command;
using Game.Battle.Objects;
using Game.Battle.Toughness;
using GameHotUpdate.Battle.Command;
using GameHotUpdate.Battle.Event.Turn;
using GameHotUpdate.Battle.Toughness;
using UnityEngine;

namespace GameHotUpdate.Objects.Battle
{
    public class RestoreToughnessState : TurnState
    {
        // 恢复速度
        private const float recoverySpeed = 55f;
        
        public RestoreToughnessState(IBattleEntityObject battleEntity) : base(battleEntity)
        {
            
        }

        public override void Enter()
        {
            BattleEntity.StartCoroutine(RestoreToughness_Cor());
        }
        
        /// <summary>
        /// 韧性恢复协程
        /// 逻辑：仅当韧性被击破时触发 → 创建韧性恢复指令 → 插入到回合队列 → 等待恢复完成
        /// </summary>
        /// <returns>协程迭代器</returns>
        private IEnumerator RestoreToughness_Cor()
        {
            // 获取当前怪物的韧性组件
            var toughnessComponent = BattleEntity.GetComponent<IToughnessComponent>();
            // 若韧性未被击破，切换为操作状态
            if (!toughnessComponent.IsToughnessBroken())
            {
                BattleEntity.ChangeState(EActPhase.Operator);
                yield break;
            }
            
            // 还是需要韧性指令
            
            float currentValue = 0;
            // 等待韧性值恢复至最大值
            while (toughnessComponent.CurrentToughnessValue < toughnessComponent.MaxToughnessVaue)
            {
                currentValue += Time.deltaTime * recoverySpeed;
                toughnessComponent.SetToughnessValue((int)currentValue, toughnessComponent.MaxToughnessVaue);
                yield return null;
            }
            
            // 切换为操作状态
            BattleEntity.ChangeState(EActPhase.Operator);
        }

        public override void Execute()
        {

        }

        public override void Exit()
        {

        }
    }
}
