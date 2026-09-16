using UnityEngine;

namespace HotUpdate.UI.Settings.Handlers
{
    /// <summary>
    /// 垂直同步处理器。
    /// 选项索引直接作为 vSyncCount：0=关闭、1=开启、2=半刷新率（每两帧一次）。
    /// 注意：开了垂直同步后，帧率上限通常会被屏幕刷新率限制。
    /// </summary>
    public class VSyncHandler : DropdownSettingHandler
    {
        public override void Execute(int optionIndex)
        {
            QualitySettings.vSyncCount = optionIndex;
        }
    }
}
