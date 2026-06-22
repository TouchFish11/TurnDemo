using HotUpdate.Game.Interact;
using UnityEngine;

namespace HotUpdate.Game.Main.FloatingText
{
    public interface IFloatingTextManager
    {
        void ClearCache();

        void AddNpc(INpcObject npcObject);
        
        void SetPlayer(Transform player);
        
        void RemoveNpc(INpcObject npcObject);
    }
}
