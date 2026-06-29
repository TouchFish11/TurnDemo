using System.Collections;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.Frostfall
{
    public class AbyssalMageFrostfallSkillPreCastPhaseStrategy : SkillPreCastPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            SkillHelper.InitMonsterSkillTarget(skill, battleCoordinator);
            // 重新初始化投射物数据
            SkillContext.ProjectileData = new ProjectileData(SkillContext.Caster, SkillContext.MainTarget, SkillContext.AllTargets, SkillContext);
            var pos = new Vector3(0, 0, -3);
            // 更新投射物变换信息
            SkillContext.ProjectileTrans = new ProjectileTrans(pos, Quaternion.identity);
            SkillContext.VFXInfo = poolManager.GetData<VFXInfo>();
            // 技能释放前短暂延迟
            yield return SkillHelper.Delay(100);
        }
    }
}
