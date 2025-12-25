using Framework;
using Game;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RoleBuilder
{
    public static async Task<PlayerObject> CreateRole(int roleId, Transform parent, bool stay = false)
    {
        return roleId switch
        {
            1 => await ObjectBuilder.GetOrCreateInstance<PlayerObject>(E_AssetBundleType.Prefab, ResKeyCollection.FireFly, parent, stay),
            2 => await ObjectBuilder.GetOrCreateInstance<PlayerObject>(E_AssetBundleType.Prefab, ResKeyCollection.Herta, parent, stay),
            _ => null,
        };
    }
}
