using System.Threading;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Config;
using Core.Loader.Sprites;
using Core.Reflection;
using Core.Service;
using Core.Tasks.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Tasks.Test
{
    public class UniTaskTest : MonoBehaviour
    {
        public Image image;
        
        private CancellationTokenSource _cancellationTokenSource;
        
        // Start is called before the first frame update
        private async void Start()
        {
            ServiceLocator.InitService();

            //await SingleTest();

            await ManagerTest();
        }

        private async Task ManagerTest()
        {
            await ServiceLocator.Get<IAssetBundleManager>().Init();
            // 初始化工厂
            ServiceLocator.Get<IFactoryManager>().InitFactorys();

            var sprite = await ServiceLocator.Get<ISpriteLoader>().LoadSpriteAsync(ResKeyCollection.Atlas_Icon_Common, ResKeyCollection.Icon_Common_Battle);
            image.sprite = sprite;

            ServiceLocator.Get<ISpriteLoader>().UnloadSpriteAsync(ResKeyCollection.Atlas_Icon_Common,
                ResKeyCollection.Icon_Common_Battle);
        }

        private async Task SingleTest()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            string path = $"{Application.streamingAssetsPath}/AssetBundles/PC.assetbundle";
            AssetBundle assetBundle = await AssetBundle.LoadFromFileAsync(path).ToTask();
            
            Debug.Log($"{assetBundle.name}，AB包是否为null：{assetBundle == null}");

            AssetBundleManifest assetBundleManifest = await assetBundle
                .LoadAssetAsync<AssetBundleManifest>(nameof(AssetBundleManifest)).ToTask<AssetBundleManifest>();
            Debug.Log($"{assetBundleManifest}，AssetBundleManifest是否为null：{assetBundleManifest == null}");

            path = $"{Application.streamingAssetsPath}/AssetBundles/texture.assetbundle";
            AssetBundle textureAb = await AssetBundle.LoadFromFileAsync(path).ToTask();
            Debug.Log($"{textureAb.name}，图片包是否为null：{textureAb == null}");
            
            Sprite sprite = await textureAb.LoadAssetAsync<Sprite>("Icon_Common_Check").ToTask<Sprite>(_cancellationTokenSource.Token);
            Debug.Log($"{sprite}，图片是否为null：{sprite == null}");
            
            image.sprite = sprite;
            
            // 卸载AB包
            await textureAb.UnloadAsync(false).ToTask();
            Debug.Log($"卸载成功");
        }
    }
}
