using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IUWRManager
{
    void LoadAssetAsync<T>(string path, UnityAction<bool, T> overCallBack) where T : class;
    void UploadAssetAsync(string url, string localPath, string fileName = null, UploadProgressCallBack progressCallBack = null);
}
