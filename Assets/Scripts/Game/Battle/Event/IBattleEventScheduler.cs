using System.Collections;
using Game.Battle.Context;
using Game.Battle.Objects;
using UnityEngine;

namespace Game.Battle.Event
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
