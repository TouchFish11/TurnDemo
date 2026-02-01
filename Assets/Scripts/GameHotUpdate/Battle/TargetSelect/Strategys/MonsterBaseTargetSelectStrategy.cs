using System.Collections.Generic;
using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.TargetSelect;
using GameHotUpdate.Battle.Context;
using UnityEngine;

namespace GameHotUpdate.Battle.TargetSelect.Strategys
{
    /// <summary>
    /// �������Ŀ��ѡ�����
    /// </summary>
    public class MonsterBaseTargetSelectStrategy : ITargetSelectStrategy
    {
        public int Priority => 0;
        private readonly List<IBattleEntityObject> players = new();

        public IBattleEntityObject SelectMainTarget(IBattleContext context, IBattleEntityObject caster, SkillInfo skillInfo)
        {
            // ���ѡ��
            players.Clear();
            context.GetAlivePlayerEntitys(players);
            var count = players.Count;
            var index = Random.Range(0, count);
            return players[index];
        }
    }
}
