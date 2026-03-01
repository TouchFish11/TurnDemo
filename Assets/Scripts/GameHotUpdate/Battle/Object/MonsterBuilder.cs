using System.Threading.Tasks;
using Core.Service;
using GameHotUpdate.Battle.Object.Monster.AbyssalMage;
using GameHotUpdate.Battle.Object.Monster.Slime;
using GameHotUpdate.Battle.Object.Monster.TurtleShell;
using GameHotUpdate.Config;
using GameHotUpdate.Main.Object;
using UnityEngine;

namespace GameHotUpdate.Battle.Object
{
    public class MonsterBuilder
    {
        public static async Task<MonsterObject> CreateMonster(int monsterId, Transform parent, bool stay = false)
        {
            return monsterId switch
            {
                1 => await ServiceLocator.Get<IObjectBuilder>().GetHotfixObject<Slime>(AbKeyCollection.Prefab, ResKeyCollection.Prefab_Slime, parent, stay),
                2 => await ServiceLocator.Get<IObjectBuilder>().GetHotfixObject<TurtleShell>(AbKeyCollection.Prefab, ResKeyCollection.Prefab_TurtleShell, parent, stay),
                4 => await ServiceLocator.Get<IObjectBuilder>().GetHotfixObject<AbyssalMage>(AbKeyCollection.Prefab, ResKeyCollection.Prefab_AbyssalMage, parent, stay),
                _ => null
            };
        }
    }
}
