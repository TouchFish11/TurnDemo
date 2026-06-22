using HotUpdate.Base.Component;

namespace HotUpdate.Game.Battle.Skill.Component
{
    /// <summary>
    /// 玩家技能组件
    /// </summary>
    [ComponentId(typeof(PlayerSkillComponent))]
    public class PlayerSkillComponent : SkillComponent
    {
        public bool IsTrigger { get; set; }
        public bool IsRelease { get; set; }
        
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
