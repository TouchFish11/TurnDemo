using System.Threading;
using Core.Log;
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
            
            _cancellationTokenSource = new CancellationTokenSource();

            string path = $"{Application.streamingAssetsPath}/AssetBundles/PC.assetbundle";
            AssetBundle assetBundle = await AssetBundle.LoadFromFileAsync(path).AsTask();
            
            Debug.Log($"{assetBundle}，AB包是否为null：{assetBundle == null}");

            AssetBundleManifest assetBundleManifest = await assetBundle
                .LoadAssetAsync<AssetBundleManifest>(nameof(AssetBundleManifest)).AsTask<AssetBundleManifest>();
            Debug.Log($"{assetBundleManifest}，AssetBundleManifest是否为null：{assetBundleManifest == null}");

            path = $"{Application.streamingAssetsPath}/AssetBundles/texture.assetbundle";
            AssetBundle textureAb = await AssetBundle.LoadFromFileAsync(path).AsTask();
            Debug.Log($"{textureAb}，图片包是否为null：{textureAb == null}");
            
            Sprite sprite = await textureAb.LoadAssetAsync<Sprite>("Icon_Common_Check").AsTask<Sprite>(_cancellationTokenSource.Token);
            Debug.Log($"{sprite}，图片是否为null：{sprite == null}");
            
            image.sprite = sprite;
            
            // 卸载AB包
            await textureAb.UnloadAsync(false).AsTask();
            Debug.Log($"卸载成功");
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
