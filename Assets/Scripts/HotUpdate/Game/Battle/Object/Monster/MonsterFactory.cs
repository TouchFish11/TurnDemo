using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster
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
        
        public async Task<IMonsterObject> CreateMonster(int monsterId, Transform parent, bool stay = false)
        {
            return monsterId switch
            {
                1 => (await _objectSpawner.SpawnAsync<Slime.Slime>(AssetKeys.Prefab_Slime, parent, worldSpace:stay)),
                2 => (await _objectSpawner.SpawnAsync<TurtleShell.TurtleShell>(AssetKeys.Prefab_TurtleShell, parent, worldSpace:stay)),
                4 => (await _objectSpawner.SpawnAsync<AbyssalMage.AbyssalMage>(AssetKeys.Prefab_AbyssalMage, parent, worldSpace:stay)),
                _ => null
            };
        }
    }
}
