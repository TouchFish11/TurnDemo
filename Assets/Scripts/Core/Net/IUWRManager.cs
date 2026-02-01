using UnityEngine.Events;

namespace Core.Net
{
    public interface IUWRManager
    {
        void LoadAssetAsync<T>(string path, UnityAction<bool, T> overCallBack) where T : class;
        void UploadAssetAsync(string url, string localPath, string fileName = null, UploadProgressCallBack progressCallBack = null);
    }
}
