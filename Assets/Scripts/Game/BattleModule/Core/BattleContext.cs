
using Framework;
using GameLogic.BattleMoudule.Entity;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.BattleMoudule.Core
{
    /// <summary>
    /// 战斗上下文
    /// </summary>
    public class BattleContext : IBattleContext
    {
        // 核心数据存储（战斗内需要全局访问的数据），所有角色（玩家、敌人、召唤物）
        private List<IBattleEntity> _allCharacters = new List<IBattleEntity>();
        // 回合管理器（核心依赖）
        private TurnManager _turnManager;
        // 自定义扩展数据（如战斗难度、场景ID）
        private Dictionary<string, object> _customData = new(); 

        public BattleContext()
        {
            // 注入自身（IBattleContext）
            _turnManager = new TurnManager(this);
        }

        public IEnumerable<IBattleEntity> GetAllBattleEntity()
        {
            return _allCharacters;
        }

        public TurnManager GetTurnManager()
        {
            return _turnManager;
        }

        /// <summary>
        /// 从预制体创建（战斗启动时，BattleManager 调用）
        /// </summary>
        /// <param name="config"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public void CreateEntity(/*object config, int ownerId*/)
        {
            // 批量创建玩家角色（从配置+预制体）
            GameObject playerObj = AssetBundleLoadManager.Instance.LoadAsset<GameObject>(E_AssetBundleType.Prefab, "TestPlayer");
            GameObject playerInstance = GameObject.Instantiate(playerObj);
            FireFly fireFly = playerInstance.GetComponent<FireFly>();

            if (fireFly != null)
            {
                int id = -1;
                // 注入上下文，供角色内部组件使用
                fireFly.Init(id, this);
                _allCharacters.Add(fireFly);
            }


            // 批量创建敌人角色
            // ...

        }

        /// <summary>
        /// 战斗初始化（启动战斗时调用）
        /// </summary>
        public void InitBattle()
        {
            // 初始化行动队列
            _turnManager.SortOrder(); 
        }

        /// <summary>
        /// 战斗结束清理（避免内存泄漏）
        /// </summary>
        public void CleanupBattle()
        {
            // 销毁所有角色 GameObject（Unity 资源清理）
            foreach (IBattleEntity entity in _allCharacters)
            {
                if (entity.GetBattleComponent<BattleCharacterComponent>(out var battleCharacterComponent))
                {
                    Object.Destroy(battleCharacterComponent.gameObject);
                }
            }
            _allCharacters.Clear();
        }
    }
}
