using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 时间检查器接口
/// </summary>
public interface ITimeChecker
{
    void AddListener(int key, UnityAction overCallBack);
    long CalcRemainTime(DateTime current, int key);
    void Clear();
    int CreateTargetTime(DateTime currentTime, int targetDay, int targetHour, int targetMin, int targetSec);
    Framework.DateTime GetDateTime(int key);
}
