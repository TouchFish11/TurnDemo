using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Mono;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.UI;
using UnityEngine;

namespace HotUpdate.UI.Battle.MonsterStateUI
{
    /// <summary>
    /// 怪物状态UI管理器
    /// 管理战斗中的角色头顶血量UI的生命周期，由战斗控制器维护唯一实例
    /// </summary>
    public class MonsterStateUIManager : IMonsterStateUIManager
    {
        [Inject] private ObjectSpawner _objectSpawner;
        [Inject] private IMonoAdapter _monoAdapter;
        
        // 怪物实体到怪物血量UI的映射
        private readonly Dictionary<IBattleEntityObject, NormalMonsterStateUI> normalMonsterStateUIs = new();

        /// <summary>
        /// 缓存怪物UI
        /// </summary>
        /// <param name="monsterObject"></param>
        /// <param name="monsterStateArea"></param>
        public async Task CreateNormalMonsterStateUI(IBattleEntityObject monsterObject, RectTransform monsterStateArea)
        {
            // 从资源包加载怪物状态UI预制体，并挂载到怪物UI区域
            var monsterStateUI = await _objectSpawner.SpawnAsync<NormalMonsterStateUI>(AssetKeys.MonsterStateUI, monsterStateArea);
            // 初始化怪物状态UI（传入战斗实体、UI挂载区域）
            await monsterStateUI.Init(monsterObject, monsterStateArea, _monoAdapter);
            normalMonsterStateUIs.Add(monsterObject, monsterStateUI);
        }

        /// <summary>
        /// 移除指定怪物UI
        /// </summary>
        /// <param name="deadMonster"></param>
        public void RemoveNormalMonsterStateUI(IBattleEntityObject deadMonster)
        {
            if (!normalMonsterStateUIs.TryGetValue(deadMonster, out var normalMonsterStateUI))
            {
                return;
            }
            
            _objectSpawner.Release(normalMonsterStateUI);
            normalMonsterStateUIs.Remove(deadMonster);
        }

        /// <summary>
        /// 激活所有怪物血量UI显示
        /// </summary>
        public void ActiveMonsterUIs()
        {
            foreach (var normalMonsterStateUI in normalMonsterStateUIs.Values)
            {
                normalMonsterStateUI.gameObject.SetActive(true);
            }
        }
        
        /// <summary>
        /// 失活所有怪物血量UI显示
        /// </summary>
        public void InActiveMonsterUIs()
        {
            foreach (var normalMonsterStateUI in normalMonsterStateUIs.Values)
            {
                normalMonsterStateUI.gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// 激活指定怪物血量UI显示
        /// 激活指定怪物UI，其它失活
        /// </summary>
        public void ActiveMonsterUI(params IBattleEntityObject[] mosters)
        {
            // 先把所有 UI 设为隐藏
            foreach (var stateUi in normalMonsterStateUIs.Values)
            {
                stateUi.gameObject.SetActive(false);
            }

            // 再把匹配的设为显示
            foreach (var monster in mosters)
            {
                if (normalMonsterStateUIs.TryGetValue(monster, out var stateUI))
                {
                    stateUI.gameObject.SetActive(true);
                }
            }
        }

        /// <summary>
        /// 失活指定怪物血量UI显示
        /// 失活指定怪物UI，其它激活
        /// </summary>
        /// <param name="mosters"></param>
        public void InActiveMonsterStateUI(params IBattleEntityObject[] mosters)
        {
            // 先把所有 UI 设为显示
            foreach (var stateUi in normalMonsterStateUIs.Values)
            {
                stateUi.gameObject.SetActive(true);
            }

            // 再把匹配的设为显示
            foreach (var monster in mosters)
            {
                if (normalMonsterStateUIs.TryGetValue(monster, out var stateUI))
                {
                    stateUI.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 移除所有血量UI
        /// </summary>
        public void RemoveAll()
        {
            foreach (var normalMonsterStateUI in normalMonsterStateUIs.Values)
            {
                _objectSpawner.Release(normalMonsterStateUI);
            }
            normalMonsterStateUIs.Clear();
            _objectSpawner.Clear();
        }

        public void Dispose()
        {
            foreach (var normalMonsterStateUI in normalMonsterStateUIs.Values)
            {
                _objectSpawner.Release(normalMonsterStateUI);
            }
            _objectSpawner.Dispose();
        }
    }
}
