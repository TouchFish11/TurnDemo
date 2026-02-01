using Core.UI.MVC;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.UI.Video
{
    /// <summary>
    /// ��Ƶ����
    /// </summary>
    public class VideoView : UIView
    {
        public void UpdateView(string key, object value)
        {
            switch (key)
            {
                case "rawImgVideo":
                    binder.GetControl<RawImage>(key).texture = value as RenderTexture;
                    break;
            }
        }
    }
}
