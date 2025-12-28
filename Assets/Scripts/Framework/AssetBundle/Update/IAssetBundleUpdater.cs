using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// AB包更新器接口
/// </summary>
public interface IAssetBundleUpdater
{
    void ChangeState(E_UpdatePhase updatePhase);
    Task<bool> CheckUpdate();
    ABUpdateContext GetContext();
    void Init();
}
