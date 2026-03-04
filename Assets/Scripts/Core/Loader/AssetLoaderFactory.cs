using Core.HotUpdate;
using Core.Loader.Audio;
using Core.Loader.Object;
using Core.Loader.Sprite;
using Core.Reflection;
using Core.Service;

namespace Core.Loader
{
    /// <summary>
    /// 资源加载器工厂
    /// 新增加载器时需要手动注册
    /// </summary>
    public class AssetLoaderFactory : Factory<IAssetLoader>
    {
        public override void InitFactory()
        {
            FactoryUtility.ScanAllType(typeToInterfaceMap, ServiceLocator.Get<IHotUpdateManager>().GetCoreAssembly());
            
            // 注册加载器到全局定位器中
            foreach (var assetLoader in typeToInterfaceMap.Values)
            {
                switch (assetLoader)
                {
                    case ISpriteLoader spriteLoader:
                        ServiceLocator.Register(spriteLoader); 
                        break;
                    case IAudioLoader audioLoader:
                        ServiceLocator.Register(audioLoader);
                        break;
                    case IPrefabLoader prefabLoader:
                        ServiceLocator.Register(prefabLoader);
                        break;
                }
            }
        }
    }
}
