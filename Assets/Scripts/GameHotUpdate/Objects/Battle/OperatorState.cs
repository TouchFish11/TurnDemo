using System.Collections;
using Game.Battle.Objects;

namespace GameHotUpdate.Objects.Battle
{
    /// <summary>
    /// 操作状态
    /// </summary>
    public abstract class OperatorState : TurnState
    {
        protected OperatorState(IBattleEntityObject battleEntity) : base(battleEntity)
        { 

        }

        public override void Enter()
        {
            BattleEntity.StartCoroutine(OnExceuteAction());
        }

        /// <summary>
        /// 执行具体行动逻辑
        /// </summary>
        /// <returns>协程迭代器（用于处理异步行动流程）</returns>
        protected abstract IEnumerator OnExceuteAction();
    }
}
