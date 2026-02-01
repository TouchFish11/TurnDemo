using Core.Loader.Loaders;
using Core.Reflection;

namespace Core.Loader
{
    /// <summary>
    /// 资源加载器接口
    /// </summary>
    public interface IAssetLoaderFactory : IFactory
    {
        /// <summary>
        /// 获取精灵加载器
        /// </summary>
        /// <returns></returns>
        ISpriteLoader GetSpriteLoader();
    }
}
