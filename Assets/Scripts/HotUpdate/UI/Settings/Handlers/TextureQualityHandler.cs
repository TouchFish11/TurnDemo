using UnityEngine;

namespace HotUpdate.UI.Settings.Handlers
{
    /// <summary>
    /// 纹理质量处理器。
    /// 选项索引即 mipmap 限制级别：0=完整、1=高、2=中、3=低（值越大，跳过越多高层 mipmap，纹理越糊）。
    /// Unity 6 里 masterTextureLimit 已改名 globalTextureMipmapLimit。
    /// </summary>
    public class TextureQualityHandler : DropdownSettingHandler
    {
        public override void Execute(int optionIndex)
        {
            QualitySettings.globalTextureMipmapLimit = optionIndex;
        }
    }
}
