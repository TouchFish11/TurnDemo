using System;
using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Log;
using Core.Mono;
using HotUpdate.Base.Manager;
using HotUpdate.Game.Dialogue;
using HotUpdate.Game.Interact;
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
        [Inject] private IPlayerManager playerManager;
        
        // 缓存注册的npc
        private readonly List<NpcObject> _npcs = new();
        // 映射索引与对应的浮动文本对象
        private readonly Dictionary<int, FloatingTextObj> npcToTextMap = new();
        // 玩家对象（用于计算距离）
        private Transform _player;
        // 浮动文本最大显示距离：超过该距离则隐藏文本
        private const float MaxDisplayDistance = 10f;
        // 全局显示状态
        private bool _globalShow = true;

        /// <summary>
        /// 玩家对象（用于计算距离）
        /// </summary>
        public Transform Player
        {
            get
            {
                if(_player)
                    return _player;
                
                if(playerManager.MainPlayer != null)
                    _player = playerManager.MainPlayer.GameObject.transform;
                else
                    return null;
                
                return _player;
            }
        }

        public FloatingTextManager(IMonoAdapter monoAdapter, IDialogueManager dialogueManager)
        {
            monoAdapter.AddFixedUpdateListener(OnFixedUpdate);
            dialogueManager.OnDialogueStart += OnOnDialogueStart;
            dialogueManager.OnDialogueEnd += OnOnDialogueEnd;
        }

        public void RegisterAndAssign(NpcObject npcObject)
        {
            _npcs.Add(npcObject);
            npcToTextMap.Add(_npcs.Count - 1, null);
        }

        public bool RemoveNpc(NpcObject npcObject)
        {
            var index = _npcs.FindIndex(n => n == npcObject);
            if(index == -1)
                return false;
            
            var remove = npcToTextMap.Remove(index, out var obj);
            _objectSpawner.Release(obj);
            return remove;
        }
        
        /// <summary>
        /// 固定更新逻辑：检测NPC与玩家距离，控制浮动文本显示/隐藏
        /// </summary>
        private void OnFixedUpdate()
        {
            if(!_globalShow)
                return;
            
            // 玩家未初始化时直接返回
            if (!Player)
                return;
            
            try
            {
                // 遍历所有需要管理的NPC
                for (var i = 0; i < _npcs.Count; i++)
                {
                    var npcObject = _npcs[i];
                    // NPC在显示距离内：显示浮动文本
                    if (Vector3.Distance(npcObject.transform.position, Player.transform.position) <= MaxDisplayDistance)
                    {
                        // 未显示文本时，创建并初始化浮动文本
                        if (!npcObject.IsShowFloatingText)
                        {
                            npcObject.IsShowFloatingText = true;
                            ShowText(i);
                        }
                    }
                    // NPC超出显示距离：隐藏浮动文本
                    else
                    {
                        // 已显示文本时，回收文本对象并移除映射
                        if (npcObject.IsShowFloatingText && npcToTextMap[i])
                        {
                            npcObject.IsShowFloatingText = false;
                            npcToTextMap[i].IsShow = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogException(ELogTags.Main, e);
            }
        }

        private async void ShowText(int index)
        {
            try
            {
                var npcObject = _npcs[index];
                var textObj = npcToTextMap[index];
                if (textObj)
                {
                    textObj.IsShow = true;
                }
                else
                {
                    // 创建浮动文本对象
                    var floatingTextObj = await _objectSpawner.SpawnAsync<FloatingTextObj>(AssetKeys.UI_3D_FloatingText);
                    // 初始化浮动文本（绑定NPC位置、玩家视角、显示名称/身份）
                    floatingTextObj.Init(npcObject.transform, Player, npcObject.NpcInfo.f_speakerName, npcObject.NpcInfo.f_identity);
                    npcToTextMap[index] = floatingTextObj;
                }
            }
            catch (Exception e)
            {
                Logger.LogException(ELogTags.Main, e);
            }
        }
        
        private void OnOnDialogueStart()
        {
            // 隐藏浮动文本显示
            _globalShow = false;
            for (var i = 0; i < _npcs.Count; i++)
            {
                var npcObject = _npcs[i];
                var textObj = npcToTextMap[i];
                if(!textObj || !npcObject.IsShowFloatingText && !npcToTextMap[i].IsShow)
                    continue;

                npcObject.IsShowFloatingText = false;
                textObj.IsShow = false;
            }
        }
        
        
        private void OnOnDialogueEnd()
        {
            _globalShow = true;
        }
        
        public void ClearCache()
        {
            // 回收所有浮动文本对象至对象池
            _objectSpawner.Release(npcToTextMap.Values);
            _objectSpawner.Clear();
            // 清空映射字典
            npcToTextMap.Clear();
            _npcs.Clear();
        }
    }
}