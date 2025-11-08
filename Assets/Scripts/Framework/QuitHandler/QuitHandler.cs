using Framework;
using System.IO;
using UnityEngine;

/// <summary>
/// 退出处理器
/// </summary>
public class QuitHandler : SingletonAutoMono<QuitHandler>
{
    /// <summary>
    /// 初始化退出处理器(Main主函数调用)
    /// </summary>
    public void ActiveHandler()
    {
        LogMgr.Instance.EnableLog = true;
        LogMgr.Log(Application.persistentDataPath);
        LogMgr.Log("退出处理器激活成功");
    }

    private void OnApplicationQuit()
    {
        //保存音乐数据
        if (GameDataMgr.Instance.MusicData != null)
            BinaryDataMgr.Instance.Save(FileUtility.LocalMusicDataFileName, GameDataMgr.Instance.MusicData);
        //保存改键数据
        if (GameDataMgr.Instance.InputActionContainer != null)
            BinaryDataMgr.Instance.Save(FileUtility.LocalInputDataFileName, GameDataMgr.Instance.InputActionContainer);
    }
}
