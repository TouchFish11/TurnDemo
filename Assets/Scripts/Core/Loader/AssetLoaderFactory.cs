using System;
using Core.Loader.Loaders;
using Core.Reflection;
using Core.Utility;

namespace Core.Loader
{
    /// <summary>
    /// 资源加载器工厂
    /// </summary>
    public class AssetLoaderFactory : Factory<IAssetLoader>, IAssetLoaderFactory
    {
        void IFactory.InitFactory()
        {
            FactoryUtility.ScanAllType(typeToInterfaceMap, AssemblyUtility.GetCoreAssembly());
        }
        
        /// <summary>
        /// 获取精灵加载器
        /// </summary>
        /// <returns></returns>
        public ISpriteLoader GetSpriteLoader()
        {
            Type loaderType = null;
#if !UNITY_EDITOR || EDITOR_TEST_AB
            loaderType = typeof(SpriteLoader);
#else
            loaderType = typeof(MockSpriteLoader);
#endif
            return typeToInterfaceMap[loaderType.ToIdentifier()] as ISpriteLoader;
        }
    }
}
