using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Õ½¶·µã
/// </summary>
public class BattlePoint : SingletonMono<BattlePoint>
{
    [SerializeField] private List<Transform> playerTrans;
    [SerializeField] private List<Transform> monsterTrans;

    public IEnumerable<Transform> GetPlayerTransforms()
    {
        return playerTrans;
    }

    public IEnumerable<Transform> GetMonsterTransforms()
    {
        return monsterTrans;
    }
}
