using System.Threading.Tasks;
using HotUpdate.Game.Battle.Context;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster
{
    public interface IMonsterFactory
    {
        Task<IMonsterObject> CreateMonster(int monsterId, int entityIndex, IBattleContext context, Transform parent, bool stay = false);
        
        void CollectDeadMonster(MonsterObject monsterObject);
    }
}
