using Game.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 目标选择管理器接口
/// </summary>
public interface ITargetSelectManager
{
    void ActiveSelectTarget();
    IBattleEntityObject GetMainTarget();
    List<IBattleEntityObject> GetTargets();
    void InActiveSelectTarget();
    void Init();
}
