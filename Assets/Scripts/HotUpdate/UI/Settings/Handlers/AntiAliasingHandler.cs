using UnityEngine;

namespace HotUpdate.UI.Settings.Handlers
{
    /// <summary>
    /// 抗锯齿处理器。
    /// 下拉「关/2x/4x/8x」对应的 MSAA 采样数不是连续的 0/1/2/3，而是 0/2/4/8，所以用查表映射索引。
    /// 注意：QualitySettings.antiAliasing 只在 Built-in 前向渲染下生效；若项目是 URP，MSAA 需在管线资产里配置，此 API 无效。
    /// </summary>
    public class AntiAliasingHandler : DropdownSettingHandler
    {
        // 选项索引 → 实际 MSAA 采样数（0=关，2/4/8=对应倍率）
        private static readonly int[] Levels = { 0, 2, 4, 8 };

        public override void Execute(int optionIndex)
        {
            if (optionIndex >= 0 && optionIndex < Levels.Length)
                QualitySettings.antiAliasing = Levels[optionIndex];
        }
    }
}
