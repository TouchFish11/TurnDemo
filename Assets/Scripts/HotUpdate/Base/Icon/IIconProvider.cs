using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HotUpdate.Base.Icon
{
    /// <summary>
    /// 图标提供器接口
    /// </summary>
    public interface IIconProvider
    {
        /// <summary>
        /// 异步加载图标
        /// </summary>
        /// <param name="iconKey"></param>
        Task<Sprite> LoadIconAsync(string iconKey);

        /// <summary>
        /// 尝试获取图标
        /// </summary>
        /// <param name="iconKey">图标Key</param>
        /// <param name="icon">获取未加载的图标返回null</param>
        /// <returns></returns>
        bool TryGetIcon(string iconKey,  out Sprite icon);

        /// <summary>
        /// 释放图标资源
        /// </summary>
        /// <param name="iconKeys"></param>
        /// <returns></returns>
        void Release(IEnumerable<string> iconKeys);

        bool Release(string iconKey);
    }
}
