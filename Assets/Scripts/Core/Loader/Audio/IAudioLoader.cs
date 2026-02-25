using System.Threading.Tasks;
using UnityEngine;

namespace Core.Loader.Audio
{
    public interface IAudioLoader : IAssetLoader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<AudioClip> LoadAudioClipAsync(string path);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        void UnloadClip(string abName, string assetName);
    }
}
