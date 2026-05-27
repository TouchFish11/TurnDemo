using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using HotUpdate.Base;

using HotUpdate.Game.Battle.Object.Role.Priest;
using HotUpdate.Game.Battle.Object.Role.Warrior;
using HotUpdate.Game.Battle.Object.Role.Wizard;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object
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
        
        public async Task<IBattleEntityObject> CreateRole(int roleId, Transform parent, bool stay = false)
        {
            return roleId switch
            {
                1 => (await _obectSpawner.SpawnAsync<Warrior>(AssetKeys.Prefab_Warrior, parent, worldSpace:stay)).Obj,
                2 => (await _obectSpawner.SpawnAsync<Wizard>(AssetKeys.Prefab_Wizard, parent, worldSpace:stay)).Obj,
                3 => (await _obectSpawner.SpawnAsync<Priest>(AssetKeys.Prefab_Priest, parent, worldSpace:stay)).Obj,
                _ => null
            };
        }
    }
}
