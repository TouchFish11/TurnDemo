using System.Collections.Generic;
using HotUpdate.Battle.Object;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.Battle.TargetSelect;
using UnityEngine;

namespace HotUpdate.Battle.TargetSelect.Strategys
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
