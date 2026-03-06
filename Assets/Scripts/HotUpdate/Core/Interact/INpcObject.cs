using UnityEngine;

namespace HotUpdate.Core.Interact
{
    public interface INpcObject
    {
        Transform Transform { get; }
        
        bool IsShowFloatingText { get; set; }
        
        NpcInfo NpcInfo { get; }

        void InitNpc(int id);
    }
}
