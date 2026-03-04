using GameHotUpdate.Battle.Object;
using UnityEngine;

namespace GameHotUpdate.Main.FloatingText
{
    public interface IFloatingTextManager
    {
        void ClearCache();

        void AddNpc(NpcObject npcObject);
        
        void SetPlayer(Transform player);
        void RemoveNpc(NpcObject npcObject);
    }
}
