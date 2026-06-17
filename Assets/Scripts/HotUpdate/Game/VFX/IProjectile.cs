using System;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.VFX
{
    public interface IProjectile
    {
        /// <summary>
        /// 初始化抛射物核心数据
        /// </summary>
        /// <param name="projectileData">抛射物配置数据</param>
        /// <param name="vFXInfo">特效配置信息</param>
        void Init(ProjectileData projectileData, VFXInfo vFXInfo);

        /// <summary>
        /// 触发时执行事件，外部用于应用效果，伤害计算、角色回能、命中特效；根据技能的逻辑触发次数，可能会多次调用
        /// </summary>
        event Action<HitResult> OnTrigger;
    }
}
