using System.Threading.Tasks;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster
{
    public interface IMonsterFactory
    {
        Task<IMonsterObject> CreateMonster(int monsterId, Transform parent, bool stay = false);
        void CollectDeadMonster(MonsterObject monsterObject);
    }
}
