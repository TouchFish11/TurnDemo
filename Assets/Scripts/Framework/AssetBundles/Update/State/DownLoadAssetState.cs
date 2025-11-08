using System.Collections;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 下载资源状态
    /// </summary>
    public class DownLoadAssetState : UpdateState
    {
        //上次速度更新时间
        private float _lastSpeedUpdateTime;
        //速度更新间隔
        private float _speedUpdateInterval;
        //是否开始下载
        private bool _isDownloading;

        public DownLoadAssetState(AssetBundleUpdateManager manager) : base(manager)
        {
            _speedUpdateInterval = GlobalSettings.Instance.SpeedUpdateInterval;
        }

        public override IEnumerator Execute()
        {
            //获取当前下载的总字节数
            long downLoadTotalBytes = assetBundleUpdateManager.GetTotalDownLoadBytes();
            //开启定期更新速度协程
            MonoManager.Instance.StartCoroutine(RegularlyUpdateSpeed());
            //正在下载AB包资源；外部需加锁累加下载字节数
            yield return assetBundleUpdateManager.DownLoadAssets(isOver => IsSuceess = isOver, (bytesPerFrame) =>
            {
                //传递进度
                assetBundleUpdateManager.TransmitDownloadProgress(bytesPerFrame, downLoadTotalBytes);
            });

            //下载结束
            _isDownloading = false;

            if (!IsSuceess)
            {
                LogMgr.LogError("资源未下载完整");
                assetBundleUpdateManager.FinishUpdate(UpdatePhase);
                yield break;
            }

            //切换为检查资源完整性状态
            assetBundleUpdateManager.ChangeState(E_UpdatePhase.CheckAssetsIntegrity);
        }

        IEnumerator RegularlyUpdateSpeed()
        {
            //初始化上次更新时间为当前时间
            _lastSpeedUpdateTime = Time.realtimeSinceStartup;
            //开始下载
            _isDownloading = true;
            while (_isDownloading)
            {
                if (Time.realtimeSinceStartup - _lastSpeedUpdateTime >= _speedUpdateInterval)
                {
                    //传递速度
                    assetBundleUpdateManager.TransmitDownloadSpeed();
                    //更新当前时间为上次更新时间
                    _lastSpeedUpdateTime = Time.realtimeSinceStartup;
                }

                yield return null;
            }
        }

        public override E_UpdatePhase UpdatePhase => E_UpdatePhase.DownLoadAssets;
    }
}
