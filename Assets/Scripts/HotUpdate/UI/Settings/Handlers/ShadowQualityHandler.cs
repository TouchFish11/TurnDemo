using UnityEngine;

namespace HotUpdate.UI.Settings.Handlers
{
    /// <summary>
    /// 阴影质量处理器。
    /// 选项索引直接对应 <see cref="ShadowQuality"/> 枚举：0=关闭(Disable)、1=硬阴影(HardOnly)、2=软阴影(All)。
    /// SO 里选项顺序需与此一致。
    /// </summary>
    public class ShadowQualityHandler : DropdownSettingHandler
    {
        public override void Execute(int optionIndex)
        {
            QualitySettings.shadows = (ShadowQuality)optionIndex;
        }
    }
}
