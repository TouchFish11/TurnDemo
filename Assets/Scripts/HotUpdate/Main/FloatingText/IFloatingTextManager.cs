using HotUpdate.Battle.Object;
using UnityEngine;

namespace HotUpdate.Main.FloatingText
{
    public interface IFloatingTextManager
    {
        void ClearCache();

        void AddNpc(NpcObject npcObject);
        
        void SetPlayer(Transform player);
        void RemoveNpc(NpcObject npcObject);
    }
}
