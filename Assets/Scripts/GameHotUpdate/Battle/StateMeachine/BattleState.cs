using System.Threading.Tasks;
using Game.Battle.Context;
using Game.Battle.Turn;
using GameHotUpdate.Turn;

namespace GameHotUpdate.Battle.StateMeachine
{
    public abstract class BattleState : IBattleState
    {
        /// <summary>
        /// 基础行动值
        /// </summary>
        private const float BASE_ACTION_VALUE = 10000f;

        /// <summary>
        /// 速度修正系数（平衡不同速度区间）
        /// </summary>
        private const float SPEED_CORRECTION = 1.0f;
        
        public IBattleStateMachine BattleStateMachine { get; private set; }
        
        public IBattleContext Context { get; private set; }
        
        protected BattleState(IBattleStateMachine battleStateMachine, IBattleContext context)
        {
            BattleStateMachine = battleStateMachine;
            Context = context;
        }
        
        /// <summary>
        /// 计算行动值
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        protected static float CalcActionValue(float speed)
        {
            // 计算行动值，基准行动值 / 速度 * 修正系数
            return BASE_ACTION_VALUE / speed * SPEED_CORRECTION;
        }

        public abstract void Enter();
        
        public abstract void Execute();
        
        public abstract void Exit();
        
        public virtual void Dispose()
        {
            BattleStateMachine = null;
            Context = null;
        }
    }
}
