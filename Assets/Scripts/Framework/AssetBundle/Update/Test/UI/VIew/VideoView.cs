using Framework;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  ”∆µΩÁ√Ê
/// </summary>
public class VideoView : UIView
{
    [System.Obsolete]
    public override void UpdateView(string key, object value)
    {
        switch (key)
        {
            case "rawImgVideo":
                binder.GetControl<RawImage>(key).texture = value as RenderTexture;
                break;
        }
    }
}
