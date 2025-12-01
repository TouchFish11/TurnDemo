using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 视频界面数据
/// </summary>
public class VideoModel : UIModel
{
    private RenderTexture rawImgVideo;

    public RenderTexture RawImgVideo
    {
        get => rawImgVideo;
        set
        {
            rawImgVideo = value;
            TriggerDataChanged(nameof(rawImgVideo), value);
        }
    }
}
