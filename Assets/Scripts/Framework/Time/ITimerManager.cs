using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 时间管理器接口
/// </summary>
public interface ITimerManager
{
    void Close();
    void ContinueTimer(int id);
    int CreateTimer(bool isRealTime, int maxTime, UnityAction timeOverCallBack, int intervalTime = 0, UnityAction intervalTimeOverCallBack = null);
    Timer GetTimer(int id);
    void PauseTimer(int id);
    void RemoveTimer(int id);
    void ResetTimer(int id);
    void SetTimeRate(E_TimeRate timeRate);
}
