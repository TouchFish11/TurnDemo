using System.Collections.Generic;
using HotUpdate.Game.Battle.Object;
using UnityEngine;

namespace HotUpdate.Game.Battle.TargetSelect.Strategys
{
    /// <summary>
    /// 怪物基础目标选择策略类
    /// </summary>
    public class MonsterBaseTargetSelectStrategy : ITargetSelectStrategy
    {
        public int Priority => 0;

        public IBattleEntityObject SelectMainTarget(List<IBattleEntityObject> targets, IBattleEntityObject caster, SkillInfo skillInfo)
        {
            var count = targets.Count;
            var index = Random.Range(0, count);
            return targets[index];
        }
    }
}
