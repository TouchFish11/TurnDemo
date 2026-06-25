using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

namespace HotUpdate.Base.Service
{
    public interface IIconService
    {
        /// <summary>
        /// 图集预加载
        /// </summary>
        /// <param name="atlasNames"></param>
        /// <returns></returns>
        Task PreLoadAtlasAsync(params string[] atlasNames);
        
        /// <summary>
        /// 异步加载图集
        /// </summary>
        /// <param name="atlasName"></param>
        /// <returns></returns>
        Task<SpriteAtlas> LoadAtlasAsync(string atlasName);
        
        /// <summary>
        /// 图片预加载
        /// </summary>
        /// <param name="spriteNames"></param>
        /// <returns></returns>
        Task PreLoadSpriteAsync(params string[] spriteNames);
        
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
