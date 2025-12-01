using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

/// <summary>
/// 视频管理器
/// </summary>
public class VideoManager : SingletonBase<VideoManager>
{
    private VideoPlayer videoPlayer;

    /// <summary>
    /// 在播放前
    /// </summary>
    public UnityAction OnPrePlay;

    /// <summary>
    /// 在播放后
    /// </summary>
    public UnityAction OnPostPlay;

    private VideoManager()
    {

    }

    public void PlayVideo(VideoClip videoClip, RenderTexture renderTexture)
    {
        // 初始化视频播放器
        if (videoPlayer == null)
        {
            GameObject videoObj = new GameObject("VideoPlayer");
            videoPlayer = videoObj.AddComponent<VideoPlayer>();

            videoPlayer.playOnAwake = false;
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        }

        videoPlayer.targetTexture = renderTexture;
        videoPlayer.clip = videoClip;
        videoPlayer.prepareCompleted += PrepareCompleted;

        // 准备
        videoPlayer.Prepare();
    }

    /// <summary>
    /// 准备完成
    /// </summary>
    /// <param name="source"></param>
    private void PrepareCompleted(VideoPlayer source)
    {
        OnPrePlay?.Invoke();
        // 播放
        source.Play();
    }
}
