using UnityEngine;

namespace HotUpdate.UI.Settings.Handlers
{
    /// <summary>
    /// 各向异性过滤处理器。
    /// Unity 的 QualitySettings.anisotropicFiltering 只有三态（Disable/Enable/ForceEnable），
    /// 并不是 2x/4x/8x/16x 逐级（真正的级别在纹理导入设置里逐纹理配置）。
    /// 这里简化映射：索引 0（关）→ Disable，其余 → Enable。若 SO 里保留了多档选项，非「关」档实际都等效 Enable。
    /// </summary>
    public class AnisotropicFilteringHandler : DropdownSettingHandler
    {
        public override void Execute(int optionIndex)
        {
            QualitySettings.anisotropicFiltering = optionIndex == 0 ? AnisotropicFiltering.Disable : AnisotropicFiltering.Enable;
        }
    }
}
