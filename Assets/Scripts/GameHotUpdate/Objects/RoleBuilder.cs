using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Config;
using Core.Service;
using Game.Battle.Objects;
using Game.Objects;
using GameHotUpdate.Objects.Roles;
using UnityEngine;

namespace GameHotUpdate.Objects
{
    public class RoleBuilder
    {
        public static async Task<IBattleEntityObject> CreateRole(int roleId, Transform parent, bool stay = false)
        {
            return roleId switch
            {
                1 => await ServiceLocator.Get<IObjectBuilder>().GetHotfixObject<FireFly>(EAssetBundleType.Prefab, ResKeyCollection.Prefab_FireFly, parent, stay),
                2 => await ServiceLocator.Get<IObjectBuilder>().GetHotfixObject<Herta>(EAssetBundleType.Prefab, ResKeyCollection.Prefab_Herta, parent, stay),
                _ => null
            };
        }
    }
}
