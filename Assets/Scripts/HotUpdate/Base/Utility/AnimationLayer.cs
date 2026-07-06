using System;
using HotUpdate.Base.Animation;

namespace HotUpdate.Base.Utility
{
    /// <summary>
    /// 动画层级
    /// </summary>
    public static class AnimationLayer
    {
        // 定义动画层级名称常量,与Animator窗口中的层级名称一致
        public const string Base_Layer_Name = "Base Layer";
        public const string Battle_Layer_Name = "Battle Layer";
        public const string Skill_Layer_Name = "Skill Layer";

        /// <summary>
        /// 层级类型枚举转状态机层级名称
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string LayerEnumToName(EAnimationLayer layer)
        {
            return layer switch
            {
                EAnimationLayer.BaseLayer => Base_Layer_Name,
                EAnimationLayer.BattleLayer => Battle_Layer_Name,
                EAnimationLayer.SkillLayer => Skill_Layer_Name,
                _ => throw new ArgumentOutOfRangeException(nameof(layer), layer, null)
            };
        }
    }
}
