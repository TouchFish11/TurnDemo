using HotUpdate.Base.Component;
using HotUpdate.Game.Battle.Object.Role;

namespace HotUpdate.Game.Battle.Skill.Component
{
    /// <summary>
    /// 玩家技能组件
    /// </summary>
    [ComponentId(typeof(PlayerSkillComponent))]
    public class PlayerSkillComponent : SkillComponent
    {
        /// <summary>
        /// 终结技是否触发，点击角色终结技图标触发
        /// </summary>
        public bool IsTrigger { get; set; }
        
        /// <summary>
        /// 终结技是否释放，按下技能按键后确认释放
        /// </summary>
        public bool IsRelease { get; set; }

        protected override void OnBattleInit()
        {
            base.OnBattleInit();
            var playerObject = (IPlayerObject)BattleEntity;
            skillComponentCore.InitSkill(((PlayerObject)BattleEntity).RoleInfo.f_skillIds, playerObject.SkillFactory);
            AddCastCondition(playerObject.DefaultCastCondition);
            AddTargetSelectStrategy(playerObject.DefaultTargetSelectStrategy);
        }

        /// <summary>
        /// 释放终结技
        /// 点击终结技技能按键后，调用该方法改变标识，触发终结技释放
        /// </summary>
        public void ReleaseUltimate()
        {
            IsRelease = true;
        }
    }
}
