using System.Collections;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Core.Battle.Event
{
    public interface IBattleEventScheduler
    {
        /// <summary>
        /// 终结技释放前调度逻辑
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skillInfo"></param>
        IEnumerator PreUltimateCastDispatch(IBattleEntityObject caster, SkillInfo skillInfo);
    }
}
