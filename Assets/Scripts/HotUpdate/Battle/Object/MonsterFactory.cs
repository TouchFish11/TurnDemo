using System.Threading.Tasks;
using Core.Loader.Object;
using Core.Reflection;
using Core.Service;
using HotUpdate.Battle.Object.Monster.AbyssalMage;
using HotUpdate.Battle.Object.Monster.Slime;
using HotUpdate.Battle.Object.Monster.TurtleShell;
using HotUpdate.Common;
using UnityEngine;

namespace HotUpdate.Battle.Object
{
    /// <summary>
    /// 怪物工厂
    /// </summary>
    public class MonsterFactory : IFactory
    {
        private static readonly IPrefabLoader _prefabLoader = ServiceLocator.Get<IPrefabLoader>();
        
        public void InitFactory()
        {
            ServiceLocator.Register(this);
        }
        
        public async Task<MonsterObject> CreateMonster(int monsterId, Transform parent, bool stay = false)
        {
            return monsterId switch
            {
                1 => await _prefabLoader.GetObjectAsync<Slime>(AbKeyCollection.Prefab, ResKeyCollection.Prefab_Slime, parent, stay),
                2 => await _prefabLoader.GetObjectAsync<TurtleShell>(AbKeyCollection.Prefab, ResKeyCollection.Prefab_TurtleShell, parent, stay),
                4 => await _prefabLoader.GetObjectAsync<AbyssalMage>(AbKeyCollection.Prefab, ResKeyCollection.Prefab_AbyssalMage, parent, stay),
                _ => null
            };
        }
    }
}
