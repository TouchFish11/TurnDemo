using GameHotUpdate.UI.MVC;
using UnityEngine;

namespace GameHotUpdate.UI.Video
{
    /// <summary>
    /// ��Ƶ��������
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
            }
        }
    }
}
