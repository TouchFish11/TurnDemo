using Core.UI.ViewController;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Game.Main.Video.UI
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
