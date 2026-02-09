using System.Collections.Generic;
using Core.Pool;
using Core.Service;
using Game.Battle.Objects;

namespace GameHotUpdate.Battle.UI.MonsterStateUI
{
    /// <summary>
    /// 怪物状态UI管理器
    /// 管理战斗中的角色头顶血量UI的生命周期，由战斗控制器维护唯一实例
    /// </summary>
    public class MonsterStateUIManager
    {
        // 怪物实体到怪物血量UI的映射
        private readonly Dictionary<IBattleEntityObject, NormalMonsterStateUI> normalMonsterStateUIs = new();
        
        /// <summary>
        /// 缓存怪物UI
        /// </summary>
        /// <param name="monsterObject"></param>
        /// <param name="normalMonsterStateUI"></param>
        public void AddNormalMonsterStateUI(IBattleEntityObject monsterObject, NormalMonsterStateUI normalMonsterStateUI)
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
            
            ServiceLocator.Get<IPoolManager>().PushObj(normalMonsterStateUI.gameObject);
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
        public void ActiveMonsterUI(IBattleEntityObject moster)
        {
            foreach (var mosterEntity in normalMonsterStateUIs.Keys)
            {
                normalMonsterStateUIs[mosterEntity].gameObject.SetActive(mosterEntity == moster);
            }
        }

        /// <summary>
        /// 失活指定怪物血量UI显示
        /// 失活指定怪物UI，其它激活
        /// </summary>
        /// <param name="moster"></param>
        public void InActiveMonsterStateUI(IBattleEntityObject moster)
        {
            foreach (var mosterEntity in normalMonsterStateUIs.Keys)
            {
                normalMonsterStateUIs[mosterEntity].gameObject.SetActive(mosterEntity != moster);
            }
        }

        /// <summary>
        /// 移除所有血量UI
        /// </summary>
        public void RemoveAllUi()
        {
            foreach (var normalMonsterStateUI in normalMonsterStateUIs.Values)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(normalMonsterStateUI.gameObject);
            }
            normalMonsterStateUIs.Clear();
        }
    }
}
