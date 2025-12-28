using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 游戏数据管理器接口
/// </summary>
public interface IGameDataManager
{
    MusicData MusicData { get; }
    MainActionMapDataContainer InputActionContainer { get; }
    InputDataContainer InputDataContainer { get; }
    TaskDataCollection TaskDataCollection { get; }

    Task InitDataAsync();
}
