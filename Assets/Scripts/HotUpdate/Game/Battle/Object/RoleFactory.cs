using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Reflection;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Common;
using HotUpdate.Game.Battle.Object.Role.Priest;
using HotUpdate.Game.Battle.Object.Role.Warrior;
using HotUpdate.Game.Battle.Object.Role.Wizard;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object
{
    /// <summary>
    /// 角色工厂
    /// </summary>
    public class RoleFactory : IFactory
    {
        [Inject] private ObjectSpawner _obectSpawner;
        
        public void InitFactory()
        {

        }
        
        public async Task<IBattleEntityObject> CreateRole(int roleId, Transform parent, bool stay = false)
        {
            return roleId switch
            {
                1 => (await _obectSpawner.SpawnAsync<Warrior>(ResKeyCollection.Prefab_Warrior, parent, worldSpace:stay)).Obj,
                2 => (await _obectSpawner.SpawnAsync<Wizard>(ResKeyCollection.Prefab_Wizard, parent, worldSpace:stay)).Obj,
                3 => (await _obectSpawner.SpawnAsync<Priest>(ResKeyCollection.Prefab_Priest, parent, worldSpace:stay)).Obj,
                _ => null
            };
        }
    }
}
