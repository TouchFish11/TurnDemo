using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Pool;
using HotUpdate.Base.Battle.Object;

namespace HotUpdate.Game.Battle.UI.MonsterStateUI
{
    /// <summary>
    /// 怪物状态UI管理器
    /// 管理战斗中的角色头顶血量UI的生命周期，由战斗控制器维护唯一实例
    /// </summary>
    public class MonsterStateUIManager
    {
        // 怪物实体到怪物血量UI的映射
        private readonly Dictionary<IBattleEntityObject, PoolObject<NormalMonsterStateUI>> normalMonsterStateUIs = new();
        
        /// <summary>
        /// 缓存怪物UI
        /// </summary>
        /// <param name="monsterObject"></param>
        /// <param name="normalMonsterStateUI"></param>
        public void AddNormalMonsterStateUI(IBattleEntityObject monsterObject, PoolObject<NormalMonsterStateUI> normalMonsterStateUI)
        {
            normalMonsterStateUIs.Add(monsterObject, normalMonsterStateUI);
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
            
            DIContainer.GetInstance<IPoolManager>().PushObj(normalMonsterStateUI.Obj.gameObject);
            normalMonsterStateUIs.Remove(deadMonster);
        }

        /// <summary>
        /// 激活所有怪物血量UI显示
        /// </summary>
        public void ActiveMonsterUIs()
        {
            foreach (var normalMonsterStateUI in normalMonsterStateUIs.Values)
            {
                normalMonsterStateUI.Obj.gameObject.SetActive(true);
            }
        }
        
        /// <summary>
        /// 失活所有怪物血量UI显示
        /// </summary>
        public void InActiveMonsterUIs()
        {
            foreach (var normalMonsterStateUI in normalMonsterStateUIs.Values)
            {
                normalMonsterStateUI.Obj.gameObject.SetActive(false);
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
                stateUi.Obj.gameObject.SetActive(false);
            }

            // 再把匹配的设为显示
            foreach (var monster in mosters)
            {
                if (normalMonsterStateUIs.TryGetValue(monster, out var stateUI))
                {
                    stateUI.Obj.gameObject.SetActive(true);
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
                stateUi.Obj.gameObject.SetActive(true);
            }

            // 再把匹配的设为显示
            foreach (var monster in mosters)
            {
                if (normalMonsterStateUIs.TryGetValue(monster, out var stateUI))
                {
                    stateUI.Obj.gameObject.SetActive(false);
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
                normalMonsterStateUI.Collect();
            }
            normalMonsterStateUIs.Clear();
        }
    }
}
