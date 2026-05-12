using UnityEngine;

namespace HotUpdate.Game.Main.Video.UI
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
