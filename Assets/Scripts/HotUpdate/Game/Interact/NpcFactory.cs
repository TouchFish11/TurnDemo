using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Serialize.Binary;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Main.FloatingText;
using UnityEngine;

namespace HotUpdate.Game.Interact
{
    /// <summary>
    /// NPC工厂
    /// </summary>
    public class NpcFactory : INpcFactory
    {
        [Inject] private IBinaryDataManager _binaryDataManager;
        [Inject] private ObjectSpawner _obectSpawner;
        [Inject] private IFloatingTextManager _floatingTextManager;
        
        public async Task<NpcObject> CreateNpc(int npcId, Vector3 position, Quaternion rotation)
        {
            var npcObject = await CreateNpcInternal(npcId);
            npcObject.transform.SetPositionAndRotation(position, rotation);
            return npcObject;
        }
        
        public async Task<NpcObject> CreateNpc(int npcId, Transform parent = null, bool stay = false)
        {
            var npcObject = await CreateNpcInternal(npcId);
            npcObject.transform.SetParent(parent, stay);
            return npcObject;
        }
        
        private async Task<NpcObject> CreateNpcInternal(int npcId)
        {
            var npcObject = npcId switch
            {
                1 => await _obectSpawner.SpawnAsync<NpcObject>(AssetKeys.Prefab_Npc),
                2 => await _obectSpawner.SpawnAsync<NpcObject>(AssetKeys.Prefab_Npc),
                _ => null
            };

            EntityHelper.InitEntity(npcObject);
            // 初始化NPC基础属性（参数为NPC配置ID，对应配置表）
            var npcInfo = _binaryDataManager.GetConfig<NpcInfoContainer>(EConfigLoadType.Excel).dataDic[npcId];
            
            // TODO：暂时写死，可通过配置获取对话的交互策略
            var strategy = DIContainer.Create<DialogueInteractStrategy>();
            npcObject.SetInteractStrategy(strategy);
            npcObject.InitNpc(npcInfo);
            _floatingTextManager.AddNpc(npcObject);
            return npcObject;
        }
    }
}
