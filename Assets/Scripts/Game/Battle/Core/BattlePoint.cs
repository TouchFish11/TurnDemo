using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗点
/// </summary>
public class BattlePoint : SingletonMono<BattlePoint>
{
    [SerializeField] private List<Transform> playerTrans;
    [SerializeField] private List<Transform> monsterTrans;

    [SerializeField] private Transform monsterPointCenter;
    [SerializeField] private List<Camera> roleCameras;

    /// <summary>
    /// 获取所有的角色位置点
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Transform> GetPlayerTransforms()
    {
        return playerTrans;
    }

    /// <summary>
    /// 获取所有的怪物位置点
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Transform> GetMonsterTransforms()
    {
        return monsterTrans;
    }

    /// <summary>
    /// 获取怪物点中心
    /// </summary>
    /// <returns></returns>
    public Transform GetMonsterPointCenter() => monsterPointCenter;
}
