using Core.UI.MVC;
using UnityEngine;

namespace GameHotUpdate.Video.UI
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
