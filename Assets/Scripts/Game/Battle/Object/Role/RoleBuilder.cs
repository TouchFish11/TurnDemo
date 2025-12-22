using Framework;
using Game;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RoleBuilder
{
    public static async Task<PlayerObject> CreateRole(int roleId, Vector3 position, Quaternion quaternion)
    {
        return roleId switch
        {
            1 => await ObjectBuilder.GetOrCreateInstance<PlayerObject>(E_AssetBundleType.Prefab, ResKeyCollection.FireFly, position, quaternion),
            2 => await ObjectBuilder.GetOrCreateInstance<PlayerObject>(E_AssetBundleType.Prefab, ResKeyCollection.Herta, position, quaternion),
            _ => null,
        };
    }
}
