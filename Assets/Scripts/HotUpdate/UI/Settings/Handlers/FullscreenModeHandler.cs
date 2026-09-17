using UnityEngine;

namespace HotUpdate.UI.Settings.Handlers
{
    /// <summary>
    /// 全屏模式处理器。
    /// 下拉选项索引直接强转成 <see cref="FullScreenMode"/> 枚举：
    /// 0=独占全屏(ExclusiveFullScreen)、1=无边框全屏(FullScreenWindow)、3=窗口化(Windowed)
    /// 所以 SO 里该设置选项的顺序必须和这个枚举顺序一致。
    /// </summary>
    public class FullscreenModeHandler : DropdownSettingHandler
    {
        public override void Execute(int optionIndex)
        {
            // TODO：不兼容MacOS
            if (optionIndex == 2)
            {
                optionIndex = 3;
            }
            Screen.fullScreenMode = (FullScreenMode)optionIndex;
        }
    }
}
