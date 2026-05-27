using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;

using HotUpdate.Game.Battle.Object.Monster.AbyssalMage;
using HotUpdate.Game.Battle.Object.Monster.Slime;
using HotUpdate.Game.Battle.Object.Monster.TurtleShell;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object
{
    /// <summary>
    /// 怪物工厂
    /// </summary>
    public class MonsterFactory : IMonsterFactory
    {
        [Inject] private ObjectSpawner _objectSpawner;
        
        public void InitFactory()
        {

        }
        
        public async Task<MonsterObject> CreateMonster(int monsterId, Transform parent, bool stay = false)
        {
            return monsterId switch
            {
                1 => (await _objectSpawner.SpawnAsync<Slime>(AssetKeys.Prefab_Slime, parent, worldSpace:stay)).Obj,
                2 => (await _objectSpawner.SpawnAsync<TurtleShell>(AssetKeys.Prefab_TurtleShell, parent, worldSpace:stay)).Obj,
                4 => (await _objectSpawner.SpawnAsync<AbyssalMage>(AssetKeys.Prefab_AbyssalMage, parent, worldSpace:stay)).Obj,
                _ => null
            };
        }
    }
}
