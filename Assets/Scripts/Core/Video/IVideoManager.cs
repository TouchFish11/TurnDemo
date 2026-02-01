using System;
using UnityEngine;
using UnityEngine.Video;

namespace Core.Video
{
    /// <summary>
    /// ��Ƶ�������ӿ�
    /// </summary>
    public interface IVideoManager
    {
        event Action OnPrePlay;
        event Action OnPostPlay;

        void PlayVideo(VideoClip videoClip, RenderTexture renderTexture);
    }
}
