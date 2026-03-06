using System.Threading.Tasks;
using Core.Loader.Object;
using Core.Reflection;
using Core.Service;
using HotUpdate.Battle.Object.Role.Priest;
using HotUpdate.Battle.Object.Role.Warrior;
using HotUpdate.Battle.Object.Role.Wizard;
using HotUpdate.Config;
using HotUpdate.Core.Battle.Object;
using UnityEngine;

namespace HotUpdate.Battle.Object
{
    /// <summary>
    /// 角色工厂
    /// </summary>
    public class RoleFactory : IFactory
    {
        private static readonly IPrefabLoader _prefabLoader = ServiceLocator.Get<IPrefabLoader>();
        
        public void InitFactory()
        {
            ServiceLocator.Register(this);
        }
        
        public async Task<IBattleEntityObject> CreateRole(int roleId, Transform parent, bool stay = false)
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
