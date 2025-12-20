using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
///  ”∆µΩÁ√Ê
/// </summary>
public class VideoView : UIView
{
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
