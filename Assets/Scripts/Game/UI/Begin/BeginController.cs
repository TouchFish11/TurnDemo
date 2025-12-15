using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 开始界面控制器工厂
/// </summary>
public class BeginControllerFactory : UIControllerFactory<BeginView, BeginModel, BeginController>
{
    public override BeginController CreateController(BeginView view, BeginModel model)
    {
        return new BeginController(view, model);
    }

    public override BeginModel CreateModel()
    {
        return new BeginModel();
    }
}

/// <summary>
/// 开始界面控制器
/// </summary>
public class BeginController : UIController<BeginView, BeginModel>
{
    public BeginController(BeginView view, BeginModel model) : base(view, model)
    {

    }

    protected override async Task OnInit()
    {
        // 注册更新事件
        AssetBundleUpdater.Instance.GetContext().OnUpdatePhase += OnUpdatePhase;
        AssetBundleUpdater.Instance.GetContext().OnProgress += OnProgress;
        AssetBundleUpdater.Instance.GetContext().OnUpdateSpeed += OnUpdateSpeed;
        AssetBundleUpdater.Instance.GetContext().OnCheckProgress += OnCheckProgress;
        AssetBundleUpdater.Instance.GetContext().OnUpdateFinish += OnUpdateFinish;

        OnUpdatePhase(E_UpdatePhase.None);
        _model.IsActiveProgress = true;
        _model.SilderProgress = 0;
        _model.TxtProgress = $"{TextUtility.FloatToStr(0, 2)}%";
        _model.TxtSize = "";
        _model.TxtSpeed = "";

        await base.OnInit();
    }

    /// <summary>
    /// 检查更新
    /// </summary>
    public async Task CheckUpdate()
    {
        if (!await AssetBundleUpdater.Instance.CheckUpdate())
        {
            LogManager.Log($"更新失败");
            return;
        }

        // 初始化AB包
        if (!await AssetBundleManager.Instance.Init())
        {
            LogManager.Log($"AB包初始化失败");
            return;
        }

        // 播放视频
        PlayVideo();
    }

    public async void PlayVideo()
    {
        VideoController videoController = await UIManager.Instance.CreateViewAsync<VideoView, VideoModel, VideoController>(E_UILayer.Mid);
        videoController.PlayVideo();
    }

    private void OnUpdatePhase(E_UpdatePhase updatePhase)
    {
        switch (updatePhase)
        {
            case E_UpdatePhase.None:
                _model.TxtPhase = "正在检查更新...";
                break;
            case E_UpdatePhase.DownLoadRemoteListFile:
                _model.TxtPhase = "正在下载清单文件...";
                break;
            case E_UpdatePhase.GetLocalCompareFile:
                _model.TxtPhase = "正在读取本地清单文件中...";
                break;
            case E_UpdatePhase.CompareContrast:
                _model.TxtPhase = "正在分析文件差异...";
                break;
            case E_UpdatePhase.DownLoadAssets:
                _model.TxtPhase = "正在下载资源...";
                break;
            case E_UpdatePhase.CheckAssetsIntegrity:
                _model.TxtPhase = "正在检查资源完整性...";
                break;
            case E_UpdatePhase.Finished:
            case E_UpdatePhase.NullState:
                _model.TxtPhase = "更新完成";
                break;
            default:
                _model.TxtPhase = "";
                LogManager.LogError($"没有实现该枚举项：{updatePhase}");
                break;
        }
    }

    private void OnProgress(long currentloadedBytes, long totalBytes)
    {
        _model.TxtSize = $"{TextUtility.ToByteUnit((ulong)currentloadedBytes)}/{TextUtility.ToByteUnit((ulong)totalBytes)}";
        _model.SilderProgress = currentloadedBytes / (float)totalBytes;
        _model.TxtProgress = $"{TextUtility.FloatToStr(_model.SilderProgress * 100, 2)}%";
    }

    private void OnCheckProgress(int current, int total)
    {
        _model.TxtProgress = $"{TextUtility.FloatToStr((current / (float)total) * 100, 2)}%";
    }

    private void OnUpdateSpeed(long currentBytes)
    {
        _model.TxtSpeed = $"{TextUtility.ToByteUnit((ulong)currentBytes)}/s";
    }

    private void OnUpdateFinish()
    {
        // 隐藏进度条
        _model.IsActiveProgress = false;
    }
}
