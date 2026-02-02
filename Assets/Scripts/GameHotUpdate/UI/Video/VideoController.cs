using Core.AssetBundles.Management;
using Core.Service;
using Core.UI;
using Core.Video;
using GameHotUpdate.UI.MVC;
using UnityEngine;
using UnityEngine.Video;

namespace GameHotUpdate.UI.Video
{
    /// <summary>
    /// ��Ƶ���������
    /// </summary>
    public class VideoController : UIController<VideoView, VideoModel>
    {
        protected override System.Threading.Tasks.Task OnInit()
        {
            return System.Threading.Tasks.Task.CompletedTask;
        }
        
        public async void PlayVideo()
        {
            // ������Ⱦ����
            RenderTexture renderTexture = await ServiceLocator.Get<IAssetBundleManager>().LoadAssetAsync<RenderTexture>(EAssetBundleType.Texture, "VideoTexture");
            // ��������
            model.RawImgVideo = renderTexture;
            // ������Ƶ
            VideoClip videoClip = await ServiceLocator.Get<IAssetBundleManager>().LoadAssetAsync<VideoClip>(EAssetBundleType.Video, "���»�İ-������ߡ�-������(����)");
            // ������Ƶ
            VideoManager.Instance.OnPrePlay += OnPrePlay;
            VideoManager.Instance.PlayVideo(videoClip, renderTexture);
        }

        private void OnPrePlay()
        {
            // ����
            ServiceLocator.Get<IUIManager>().DestroyView(this);
        }
    }
}
