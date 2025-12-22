using Framework;
using Game;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MonsterBuilder
{
    public static async Task<MonsterObject> CreateMonster(int monsterId, Vector3 position, Quaternion quaternion)
    {
        return monsterId switch
        {
            1 => await ObjectBuilder.GetOrCreateInstance<MonsterObject>(E_AssetBundleType.Prefab, ResKeyCollection.TestMonster, position, quaternion),
            _ => null,
        };
    }
}
