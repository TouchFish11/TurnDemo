using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 日志管理器接口
/// </summary>
public interface ILogManager
{
    bool EnableLog { get; set; }

    void UploadLog(UploadProgressCallBack progressCallBack);
}
