using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Config;
using Core.Service;
using Game.Battle.Objects;
using Game.Objects;
using GameHotUpdate.Battle.Object.Role.Priest;
using GameHotUpdate.Battle.Object.Role.Warrior;
using GameHotUpdate.Battle.Object.Role.Wizard;
using UnityEngine;

namespace GameHotUpdate.Battle.Object
{
    public class RoleBuilder
    {
        public static async Task<IBattleEntityObject> CreateRole(int roleId, Transform parent, bool stay = false)
        {
            return roleId switch
            {
                1 => await ServiceLocator.Get<IObjectBuilder>().GetHotfixObject<Warrior>(EAssetBundleType.Prefab, ResKeyCollection.Prefab_Warrior, parent, stay),
                2 => await ServiceLocator.Get<IObjectBuilder>().GetHotfixObject<Wizard>(EAssetBundleType.Prefab, ResKeyCollection.Prefab_Wizard, parent, stay),
                3 => await ServiceLocator.Get<IObjectBuilder>().GetHotfixObject<Priest>(EAssetBundleType.Prefab, ResKeyCollection.Prefab_Priest, parent, stay),
                _ => null
            };
        }
    }
}
