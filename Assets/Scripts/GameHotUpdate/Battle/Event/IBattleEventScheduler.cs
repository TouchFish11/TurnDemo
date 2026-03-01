using System.Collections;
using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Object;
using UnityEngine;

namespace GameHotUpdate.Battle.Event
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
