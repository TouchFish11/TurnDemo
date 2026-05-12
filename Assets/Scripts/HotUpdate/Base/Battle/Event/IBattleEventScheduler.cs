using System.Collections;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Common.Config.ExcelInfo.Info;

namespace HotUpdate.Base.Battle.Event
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
