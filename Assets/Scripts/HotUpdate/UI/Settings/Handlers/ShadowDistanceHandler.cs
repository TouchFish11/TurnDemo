using UnityEngine;

namespace HotUpdate.UI.Settings.Handlers
{
    /// <summary>
    /// 阴影距离处理器。
    /// 滑块值（已经过 DisplayMultiplier 换算回原始值）直接作为阴影渲染距离。
    /// 距离越大，远处物体也投影阴影，但性能越差。
    /// </summary>
    public class ShadowDistanceHandler : SliderSettingHandler
    {
        public override void Excute(float sliderValue)
        {
            QualitySettings.shadowDistance = sliderValue;
        }
    }
}
