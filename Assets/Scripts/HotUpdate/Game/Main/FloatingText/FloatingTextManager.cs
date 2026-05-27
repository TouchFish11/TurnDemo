using System;
using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Mono;
using HotUpdate.Base.Interact;

using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Main.FloatingText
{
    /// <summary>
    /// 浮动文本管理器：负责NPC浮动文本的显示/隐藏管理
    /// </summary>
    public class FloatingTextManager : IFloatingTextManager
    {
        [Inject] private ObjectSpawner _objectSpawner;

        // 存储需要显示浮动文本的NPC列表
        private readonly List<INpcObject> npcObjects = new();
        // 映射NPC与对应的浮动文本对象，便于快速查找和管理
        private readonly Dictionary<INpcObject, PoolObject<FloatingTextObj>> npcToTextMap = new();
        // 玩家对象（用于计算距离）
        private Transform player;
        // 浮动文本最大显示距离：超过该距离则隐藏文本
        private const float MaxDisplayDistance = 10f;

        public FloatingTextManager(IMonoAdapter monoAdapter)
        {
            monoAdapter.AddFixedUpdateListener(OnFixedUpdate);
        }

        /// <summary>
        /// 添加需要管理浮动文本的NPC
        /// </summary>
        /// <param name="npcObject">目标NPC对象</param>
        public void AddNpc(INpcObject npcObject)
        {
            npcObjects.Add(npcObject);
        }

        public void RemoveNpc(INpcObject npcObject)
        {
            npcObjects.Remove(npcObject);
        }

        public void SetPlayer(Transform player)
        {
            this.player = player;
        }

        /// <summary>
        /// 固定更新逻辑：检测NPC与玩家距离，控制浮动文本显示/隐藏
        /// </summary>
        private async void OnFixedUpdate()
        {
            // 玩家未初始化时直接返回
            if (player == null)
            {
                return;
            }
            
            try
            {
                // 遍历所有需要管理的NPC
                foreach (var npcObject in npcObjects)
                {
                    // NPC在显示距离内：显示浮动文本
                    if (Vector3.Distance(npcObject.Transform.position, player.transform.position) <= MaxDisplayDistance)
                    {
                        // 未显示文本时，创建并初始化浮动文本
                        if (!npcObject.IsShowFloatingText)
                        {
                            npcObject.IsShowFloatingText = true;
                            
                            // 从对象池/资源加载浮动文本对象
                            var floatingTextObj = await _objectSpawner.SpawnAsync<FloatingTextObj>(AssetKeys.UI_3D_FloatingText);
                            // 初始化浮动文本（绑定NPC位置、玩家视角、显示名称/身份）
                            floatingTextObj.Obj.Init(npcObject.Transform, player, npcObject.NpcInfo.f_speakerName, npcObject.NpcInfo.f_identity);
                            // 将NPC与文本对象映射存储
                            npcToTextMap.TryAdd(npcObject, floatingTextObj);
                        }
                    }
                    // NPC超出显示距离：隐藏浮动文本
                    else
                    {
                        // 已显示文本时，回收文本对象并移除映射
                        if (npcObject.IsShowFloatingText)
                        {
                            npcObject.IsShowFloatingText = false;
                            // 将文本对象回收至对象池
                            npcToTextMap[npcObject].Collect();
                            // 移除NPC与文本的映射关系
                            npcToTextMap.Remove(npcObject);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(FloatingTextManager)}.{nameof(OnFixedUpdate)}：{e.Message}，{e.StackTrace}");
            }
        }

        /// <summary>
        /// 清理缓存：清空所有NPC和浮动文本映射，回收文本对象
        /// </summary>
        public void ClearCache()
        {
            // 清空NPC列表
            npcObjects.Clear();
            // 回收所有浮动文本对象至对象池
            foreach (var text in npcToTextMap.Values)
            {
                text.Collect();
            }
            _objectSpawner.Dispose();
            // 清空映射字典
            npcToTextMap.Clear();
            // 重置玩家引用
            player = null;
        }
    }
}