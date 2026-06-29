using System.Collections;
using System.Threading.Tasks;
using Core.DI;
using Core.Pool;
using Core.Time;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Statuses;
using HotUpdate.Game.VFX;

namespace HotUpdate.Game.Battle.Skill.Base.Flow
{
    /// <summary>
    /// 技能事件处理策略
    /// </summary>
    public abstract class SkillEventProcessPhaseStrategy : SkillPhaseStrategy
    {
        // 状态工厂
        [Inject] protected IStatusFactory statusFactory;
        // 伤害计算管理器
        [Inject] protected IDamageCalcManager damageCalcManager;
        // 特效管理器
        [Inject] protected IVFXManager vfxManager;
        // 计时器管理器
        [Inject] protected ITimerManager timerManager;
        // 对象池管理器
        [Inject] protected IPoolManager poolManager;

        /// <summary>
        /// 是否正在处理技能事件
        /// </summary>
        public bool IsProcessing { get; protected set; } = true;

        /// <summary>
        /// 不使用该逻辑来等待处理完成
        /// </summary>
        /// <returns></returns>
        public sealed override IEnumerator Execute()
        {
            yield break;
        }

        /// <summary>
        /// 处理事件回调
        /// </summary>
        /// <param name="result"></param>
        public async void ProcessEvent(HitResult result)
        {
            await OnTrigger(result);
            IsProcessing = false;
        }

        /// <summary>
        /// 技能弹射物触发事件时的处理逻辑
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected abstract Task OnTrigger(HitResult result);
        
        public void Reset()
        {
            IsProcessing = true;
        }
    }
}
