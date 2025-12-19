
using Framework;
using Game.Main;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 战斗上下文
    /// </summary>
    public class BattleContext : IBattleContext
    {
        // 战斗事件总线实例
        private readonly BattleEventBus eventBus;
        // 核心数据存储（战斗内需要全局访问的数据），所有角色（玩家、敌人、召唤物）
        private readonly List<IBattleEntityObject> _allCharacters = new List<IBattleEntityObject>();
        // 回合管理器（核心依赖）
        private readonly TurnManager _turnManager;
        // 自定义扩展数据（如战斗难度、场景ID）
        private Dictionary<string, object> _customData = new(); 

        public BattleContext()
        {
            eventBus = new BattleEventBus();
            // 注入自身（IBattleContext）
            _turnManager = new TurnManager(this);
        }

        /// <summary>
        /// 战斗初始化（启动战斗时调用）
        /// </summary>
        public async Task InitBattle()
        {
            // 创建战斗对象
            await CreateBattleEntity();
            // 初始化行动队列
            _turnManager.SortOrder();
        }

        /// <summary>
        /// 创建战斗实体对象
        /// </summary>
        /// <param name="config"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        private async Task CreateBattleEntity(/*object config, int ownerId*/)
        {
            var playerTrans = BattlePoint.Instance.GetPlayerTransforms();
            // 批量创建玩家角色（从配置+预制体）
            foreach (Transform transform in playerTrans)
            {
                PlayerObject playerObject = await ObjectBuilder.GetOrCreateInstance<PlayerObject>(E_AssetBundleType.Prefab, ResKeyCollection.TestPlayer, transform.position, transform.rotation);
                // 注入上下文，供角色内部组件使用
                playerObject.BattleInit(-1, this);
                _allCharacters.Add(playerObject);
            }

            // 批量创建敌人角色
            var monsterTrans = BattlePoint.Instance.GetMonsterTransforms();
            // 批量创建玩家角色（从配置+预制体）
            foreach (Transform transform in monsterTrans)
            {
                MonsterObject monsterObject = await ObjectBuilder.GetOrCreateInstance<MonsterObject>(E_AssetBundleType.Prefab, ResKeyCollection.TestPlayer, transform.position, transform.rotation);
                // 注入上下文，供角色内部组件使用
                monsterObject.BattleInit(-1, this);
                _allCharacters.Add(monsterObject);
            }
        }

        /// <summary>
        /// 战斗结束清理（避免内存泄漏）
        /// </summary>
        public void CleanupBattle()
        {
            // 销毁所有角色 GameObject（Unity 资源清理）
            foreach (IBattleEntityObject entity in _allCharacters)
            {
                Object.Destroy(entity.GameObject);
            }
            _allCharacters.Clear();
        }

        public IEnumerable<IBattleEntityObject> GetAllBattleEntity()
        {
            return _allCharacters;
        }

        public TurnManager GetTurnManager()
        {
            return _turnManager;
        }

        public BattleEventBus GetEventBus()
        {
            return eventBus;
        }
    }
}
