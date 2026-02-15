using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Config;
using Core.Service;
using Game.Objects;
using GameHotUpdate.Battle.Object.Monster.Slime;
using GameHotUpdate.Battle.Object.Monster.TurtleShell;
using UnityEngine;

namespace GameHotUpdate.Battle.Object
{
    public class MonsterBuilder
    {
        public static async Task<MonsterObject> CreateMonster(int monsterId, Transform parent, bool stay = false)
        {
            return monsterId switch
            {
                1 => await ServiceLocator.Get<IObjectBuilder>().GetHotfixObject<Slime>(EAssetBundleType.Prefab, ResKeyCollection.Prefab_Slime, parent, stay),
                2 => await ServiceLocator.Get<IObjectBuilder>().GetHotfixObject<TurtleShell>(EAssetBundleType.Prefab, ResKeyCollection.Prefab_TurtleShell, parent, stay),
                _ => null
            };
        }
    }
}
