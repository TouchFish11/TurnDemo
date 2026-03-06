using HotUpdate.Core.Interact;
using UnityEngine;

namespace HotUpdate.Core.Main
{
    public interface IFloatingTextManager
    {
        void ClearCache();

        void AddNpc(INpcObject npcObject);
        
        void SetPlayer(Transform player);
        
        void RemoveNpc(INpcObject npcObject);
    }
}
