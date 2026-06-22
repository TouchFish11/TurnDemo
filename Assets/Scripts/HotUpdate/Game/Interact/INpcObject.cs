using HotUpdate.Common.Config.ExcelInfo.Info;
using UnityEngine;

namespace HotUpdate.Game.Interact
{
    public interface INpcObject
    {
        Transform Transform { get; }
        
        bool IsShowFloatingText { get; set; }
        
        NpcInfo NpcInfo { get; }

        /// <summary>
        /// 初始化NPC
        /// </summary>
        /// <param name="npcConfigId"></param>
        void InitNpc(int npcConfigId);
    }
}
