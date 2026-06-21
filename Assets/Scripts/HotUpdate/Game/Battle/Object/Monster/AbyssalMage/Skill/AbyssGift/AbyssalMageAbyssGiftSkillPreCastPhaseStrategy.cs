using System.Collections;
using Core.DI;
using Core.Pool;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.AbyssGift
{
    public class AbyssalMageAbyssGiftSkillPreCastPhaseStrategy : SkillPreCastPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            SkillHelper.InitSkillTarget(skill, battleCoordinator);
            
            // 重新初始化投射物数据
            SkillContext.ProjectileData = new ProjectileData(SkillContext.Caster, SkillContext.MainTarget, SkillContext.AllTargets, SkillContext);
            // 更新投射物变换信息
            SkillContext.ProjectileTrans = new ProjectileTrans(SkillContext.Caster.GameObject.transform.position, Quaternion.identity);
            SkillContext.VFXInfo = DIContainer.GetInstance<IPoolManager>().GetData<VFXInfo>();
            
            // 技能释放前短暂延迟
            yield return SkillHelper.Delay(100);
        }
    }
}
