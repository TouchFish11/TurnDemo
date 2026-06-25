using System.Threading.Tasks;
using UnityEngine;

namespace HotUpdate.Game.Interact
{
    public interface INpcFactory
    {
        Task<NpcObject> CreateNpc(int npcId, Vector3 position, Quaternion rotation);
        
        Task<NpcObject> CreateNpc(int npcId, Transform parent = null, bool stay = false);
    }
}
