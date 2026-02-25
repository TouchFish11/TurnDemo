using System.Threading;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Loader.Sprite;
using Core.Loader.UI;
using Core.Log;
using Core.Reflection;
using Core.Service;
using Core.Tasks.Extensions;
using Core.UI;
using GameHotUpdate.Config;
using GameHotUpdate.UI.Back;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.Test
{
    public class UniTaskTest : MonoBehaviour
    {
        public Image image;
        
        public Transform canvas;
        
        private CancellationTokenSource _cancellationTokenSource;
        
        // Start is called before the first frame update
        private async void Start()
        {
            ServiceLocator.InitService();
            await ServiceLocator.Get<IAssetBundleManager>().Init();
            // 初始化工厂
            ServiceLocator.Get<IFactoryManager>().InitHotFactorys();
            //ServiceLocator.Register<IUIManager>(UIManager.Instance);

            var backView = await ServiceLocator.Get<IUiLoader>().GetUIObject<BackView>(AbKeyCollection.Ui, ResKeyCollection.BackView, canvas);
            LogManager.Log($"{backView}");
            
            await ServiceLocator.Get<IUIManager>().InitUIManagerAsync("TODO", "TODO", "TODO");

            await ServiceLocator.Get<IUIManager>().CreateViewAsync<BackView, BackModel, BackController>(AbKeyCollection.Ui, E_UILayer.Bot, ResKeyCollection.BackView);

            //await SingleTest();

            //await ManagerTest();
        }

        private async Task ManagerTest()
        {
            await ServiceLocator.Get<IAssetBundleManager>().Init();
            // 初始化工厂
            ServiceLocator.Get<IFactoryManager>().InitHotFactorys();
            
            var sprite = await ServiceLocator.Get<ISpriteLoader>().LoadSpriteAsync("TODO", ResKeyCollection.Atlas_Icon_Common, ResKeyCollection.Icon_Common_Battle);
            image.sprite = sprite;

            ServiceLocator.Get<ISpriteLoader>().UnloadSpriteAsync(AbKeyCollection.Spriteatlas, ResKeyCollection.Atlas_Icon_Common,
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
