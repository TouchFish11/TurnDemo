using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Log;
using Core.Service;
using Core.Tasks.Extensions;
using UnityEngine;

namespace Core.Loader.Audios
{
    /// <summary>
    /// 音频加载器
    /// </summary>
    public class AudioLoader : IAudioLoader
    {
        // 音频缓存
        private readonly Dictionary<string, AudioData> _audioDatas =  new();
        
        public async Task<AudioClip> LoadAudioClipAsync(string assetName)
        {
            if (_audioDatas.TryGetValue(assetName, out var data))
            {
                return data.AudioClip;
            }
            
            // 加载音频包
            var assetBundle = await ServiceLocator.Get<IAssetBundleManager>().LoadBundleAsync(EAssetBundleType.Music);
            // 加载音频资源
            var audioClip = await assetBundle.LoadAssetAsync<AudioClip>(assetName).ToTask<AudioClip>();

            if (!audioClip)
            {
                LogManager.LogWarning($"{nameof(AudioLoader)}.{nameof(LoadAudioClipAsync)}，音频：{assetName}，加载失败，返回null");
                return null;
            }
            
            // 缓存音频数据
            _audioDatas.Add(assetName, new AudioData(audioClip));
            return audioClip;
        }

        public void UnloadClip(string assetName)
        {
            if (!_audioDatas.TryGetValue(assetName, out var data))
            {
                return;
            }
            // 卸载
            data.Unload();

            if (data.RefCount != 0)
            {
                return;
            }
            
            // 卸载音频包
            ServiceLocator.Get<IAssetBundleManager>().UnloadBundleAsync(EAssetBundleType.Music);
            _audioDatas.Remove(assetName);
        }
    }
}
