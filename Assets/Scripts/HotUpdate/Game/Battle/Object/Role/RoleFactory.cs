using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using HotUpdate.Base.Utility;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role
{
    /// <summary>
    /// 角色工厂
    /// </summary>
    public class RoleFactory : IRoleFactory
    {
        [Inject] private ObjectSpawner _obectSpawner;
        
        public void InitFactory()
        {

        }
        
        public async Task<IPlayerObject> CreateRole(int roleId, Transform parent, bool stay = false)
        {
            IPlayerObject playerObject = roleId switch
            {
                1 => await _obectSpawner.SpawnAsync<Warrior.Warrior>(AssetKeys.Prefab_Warrior, parent, worldSpace:stay),
                2 => await _obectSpawner.SpawnAsync<Wizard.Wizard>(AssetKeys.Prefab_Wizard, parent, worldSpace:stay),
                3 => await _obectSpawner.SpawnAsync<Priest.Priest>(AssetKeys.Prefab_Priest, parent, worldSpace:stay),
                _ => null
            };

            return EntityHelper.InitEntity(playerObject);
        }
    }
}
