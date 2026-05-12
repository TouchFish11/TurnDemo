using System;
using UnityEngine;
using UnityEngine.Video;

namespace Core.Video
{
    /// <summary>
    /// 视频播放管理器
    /// </summary>
    public class VideoPlayManager : IVideoManager
    {
        private VideoPlayer videoPlayer;

        public event Action OnPrePlay;

        /// <summary>
        /// 
        /// </summary>
        public event Action OnPostPlay;

        private VideoPlayManager()
        {

        }
        
        public void PlayVideo(VideoClip videoClip, RenderTexture renderTexture)
        {

            if (!videoPlayer)
            {
                var videoObj = new GameObject("VideoPlayer");
                videoPlayer = videoObj.AddComponent<VideoPlayer>();

                videoPlayer.playOnAwake = false;
                videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            }

            videoPlayer.targetTexture = renderTexture;
            videoPlayer.clip = videoClip;
            videoPlayer.prepareCompleted += PrepareCompleted;
            
            videoPlayer.Prepare();
        }
        
        private void PrepareCompleted(VideoPlayer source)
        {
            OnPrePlay?.Invoke();
            source.Play();
        }
    }
}
