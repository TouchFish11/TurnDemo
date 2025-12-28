using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// 视频管理器接口
/// </summary>
public interface IVideoManager
{
    event Action OnPrePlay;
    event Action OnPostPlay;

    void PlayVideo(VideoClip videoClip, RenderTexture renderTexture);
}
