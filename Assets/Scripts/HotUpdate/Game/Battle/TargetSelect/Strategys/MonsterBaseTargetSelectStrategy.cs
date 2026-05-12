using System.Collections.Generic;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Base.Battle.TargetSelect;
using HotUpdate.Common.Config.ExcelInfo.Info;
using UnityEngine;

namespace HotUpdate.Game.Battle.TargetSelect.Strategys
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
