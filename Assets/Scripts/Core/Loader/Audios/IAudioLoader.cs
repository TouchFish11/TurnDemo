using System.Threading.Tasks;
using UnityEngine;

namespace Core.Loader.Audios
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
        /// <param name="assetName"></param>
        /// <returns></returns>
        void UnloadClip(string assetName);
    }
}
