using HotUpdate.Base.Interact;
using UnityEngine;

namespace HotUpdate.Base.Main
{
    public interface IFloatingTextManager
    {
        void ClearCache();

        void AddNpc(INpcObject npcObject);
        
        void SetPlayer(Transform player);
        
        void RemoveNpc(INpcObject npcObject);
    }
}
