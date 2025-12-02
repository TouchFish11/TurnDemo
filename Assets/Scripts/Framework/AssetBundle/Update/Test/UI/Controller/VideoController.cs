using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// 视频界面控制器工厂
/// </summary>
public class VideoControllerFactory : UIControllerFactory<VideoView, VideoModel, VideoController>
{
    public override VideoController CreateController(VideoView view, VideoModel model)
    {
        return new VideoController(view, model);
    }

    public override VideoModel CreateModel()
    {
        return new VideoModel();
    }
}

/// <summary>
/// 视频界面控制器
/// </summary>
public class VideoController : UIController<VideoView, VideoModel>
{
    public VideoController(VideoView view, VideoModel model) : base(view, model)
    {

    }

    protected override void OnInit()
    {

    }

    public async void PlayVideo()
    {
        // 显示黑背景界面
        await UIManager.Instance.ShowViewAsync<BackView, BackModel, BackController>(E_UILayer.Mid);
        // 加载渲染纹理
        RenderTexture renderTexture = await AssetBundleManager.Instance.LoadAssetAsync<RenderTexture>(E_AssetBundleType.Texture, "VideoTexture");
        // 设置纹理
        _model.RawImgVideo = renderTexture;
        // 加载视频
        VideoClip videoClip = await AssetBundleManager.Instance.LoadAssetAsync<VideoClip>(E_AssetBundleType.Video, "六月花陌-《面壁者》-邓紫棋(高清)");
        // 播放视频
        VideoManager.Instance.OnPrePlay += OnPrePlay;
        VideoManager.Instance.PlayVideo(videoClip, renderTexture);
    }

    private void OnPrePlay()
    {
        // 隐藏
        UIManager.Instance.HideView<BackView, BackModel, BackController>();
    }
}
