using System.Threading.Tasks;
using Core.Loader.Object;
using Core.Service;
using GameHotUpdate.Battle.Object.Role.Priest;
using GameHotUpdate.Battle.Object.Role.Warrior;
using GameHotUpdate.Battle.Object.Role.Wizard;
using GameHotUpdate.Config;
using UnityEngine;

namespace GameHotUpdate.Battle.Object
{
    public class RoleBuilder
    {
        private static readonly IPrefabLoader _prefabLoader = ServiceLocator.Get<IPrefabLoader>();
        
        public static async Task<IBattleEntityObject> CreateRole(int roleId, Transform parent, bool stay = false)
        {
            return roleId switch
            {
                1 => await _prefabLoader.GetObjectAsync<Warrior>(AbKeyCollection.Prefab, ResKeyCollection.Prefab_Warrior, parent, stay),
                2 => await _prefabLoader.GetObjectAsync<Wizard>(AbKeyCollection.Prefab, ResKeyCollection.Prefab_Wizard, parent, stay),
                3 => await _prefabLoader.GetObjectAsync<Priest>(AbKeyCollection.Prefab, ResKeyCollection.Prefab_Priest, parent, stay),
                _ => null
            };
        }
    }
}
