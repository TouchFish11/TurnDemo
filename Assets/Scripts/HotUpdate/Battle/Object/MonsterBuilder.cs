using System.Threading.Tasks;
using Core.Loader.Object;
using Core.Service;
using HotUpdate.Battle.Object.Monster.AbyssalMage;
using HotUpdate.Battle.Object.Monster.Slime;
using HotUpdate.Battle.Object.Monster.TurtleShell;
using HotUpdate.Config;
using UnityEngine;

namespace HotUpdate.Battle.Object
{
    public class MonsterBuilder
    {
        private static readonly IPrefabLoader _prefabLoader = ServiceLocator.Get<IPrefabLoader>();
        
        public static async Task<MonsterObject> CreateMonster(int monsterId, Transform parent, bool stay = false)
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
