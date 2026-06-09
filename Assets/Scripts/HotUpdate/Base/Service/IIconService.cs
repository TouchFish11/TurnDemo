using System.Threading.Tasks;
using UnityEngine;

namespace HotUpdate.Base.Service
{
    public interface IIconService
    {
        /// <summary>
        /// 异步加载图片
        /// </summary>
        /// <param name="iconKey"></param>
        /// <returns></returns>
        Task<Sprite> LoadIconAsync(string iconKey);
        
        /// <summary>
        /// 尝试获取已有图片
        /// </summary>
        /// <param name="iconKey"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        bool TryGetIcon(string iconKey,  out Sprite icon);
        
        /// <summary>
        /// 释放制定图片资源
        /// </summary>
        /// <param name="iconKey"></param>
        /// <returns></returns>
        bool Release(string iconKey);
        
        /// <summary>
        /// 释放所有加载的图片
        /// </summary>
        void ReleaseAll();
    }
}
