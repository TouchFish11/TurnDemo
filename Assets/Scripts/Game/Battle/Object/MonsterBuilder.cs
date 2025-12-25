using Framework;
using Game;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MonsterBuilder
{
    public static async Task<MonsterObject> CreateMonster(int monsterId, Transform parent, bool stay = false)
    {
        return monsterId switch
        {
            1 => await ObjectBuilder.GetOrCreateInstance<MonsterObject>(E_AssetBundleType.Prefab, ResKeyCollection.Slime, parent, stay),
            2 => await ObjectBuilder.GetOrCreateInstance<MonsterObject>(E_AssetBundleType.Prefab, ResKeyCollection.TurtleShell, parent, stay),
            _ => null,
        };
    }
}
