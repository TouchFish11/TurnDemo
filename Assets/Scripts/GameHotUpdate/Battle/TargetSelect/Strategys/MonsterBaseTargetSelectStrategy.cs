using System.Collections.Generic;
using Game.Battle.Objects;
using Game.Battle.TargetSelect;
using UnityEngine;

namespace GameHotUpdate.Battle.TargetSelect.Strategys
{
    /// <summary>
    /// �������Ŀ��ѡ�����
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
