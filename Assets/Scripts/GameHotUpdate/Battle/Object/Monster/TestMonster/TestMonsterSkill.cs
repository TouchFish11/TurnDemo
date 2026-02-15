using System.Collections;
using Core.Log;
using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Status;
using GameHotUpdate.Battle.Skill.Base;

namespace GameHotUpdate.Battle.Object.Monster.TestMonster
{
    public class TestMonsterSkill : MonsterSkill
    {
        public TestMonsterSkill(IBattleEntityObject caster, int skillId, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, statusAddStrategy)
        {

        }
        
        protected override IEnumerator OnCast(IBattleContext context)
        {
            LogManager.Log($"{Caster.GameObject.name}�ͷż��ܣ�{SkillInfo.f_name}");
            yield break;
        }

        protected override void InitProjectile()
        {
            
        }
    }
}
