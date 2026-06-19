using System.Collections;
using System.Threading.Tasks;
using Core.DI;
using Core.Time;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Status;
using HotUpdate.Game.VFX;

namespace HotUpdate.Game.Battle.Skill.Base.Flow
{
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

        /// <summary>
        /// 是否正在处理技能事件
        /// </summary>
        public bool IsProcessing { get; protected set; } = true;

        public sealed override IEnumerator Execute()
        {
            yield break;
        }

        public async void ProcessEvent(HitResult result)
        {
            await OnTrigger(result);
            IsProcessing = false;
        }

        protected abstract Task OnTrigger(HitResult result);
        
        public void Reset()
        {
            IsProcessing = true;
        }
    }
}
