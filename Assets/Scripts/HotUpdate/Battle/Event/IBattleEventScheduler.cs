using System.Collections;
using HotUpdate.Battle.Context;
using HotUpdate.Battle.Object;
using UnityEngine;

namespace HotUpdate.Battle.Event
{
    public interface IBattleEventScheduler
    {
        GameObject GameObject { get; }
        
        void Init(IBattleContext context);

        /// <summary>
        /// 终结技释放前调度逻辑
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skillInfo"></param>
        IEnumerator PreUltimateCastDispatch(IBattleEntityObject caster, SkillInfo skillInfo);
    }
}
