using HotUpdate.Common.Config.ExcelInfo.Info;
using UnityEngine;

namespace HotUpdate.Base.Interact
{
    public interface INpcObject
    {
        Transform Transform { get; }
        
        bool IsShowFloatingText { get; set; }
        
        NpcInfo NpcInfo { get; }

        void InitNpc(int id);
    }
}
