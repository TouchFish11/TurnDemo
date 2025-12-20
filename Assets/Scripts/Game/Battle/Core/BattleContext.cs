using Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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
            List<Transform> playerTrans = new List<Transform>(BattlePoint.Instance.GetPlayerTransforms());
            // 批量创建玩家角色（从配置+预制体）
            var playerDataDic = BinaryDataMgr.Instance.GetConfig<RoleInfoContainer>(E_ConfigLoadType.Editor).dataDic;
            int index = 0;
            foreach (int roleId in playerDataDic.Keys)
            {
                if (index == playerTrans.Count)
                {
                    break;
                }

                Transform transform = playerTrans[index];
                PlayerObject playerObject = await ObjectBuilder.GetOrCreateInstance<PlayerObject>(E_AssetBundleType.Prefab, ResKeyCollection.TestPlayer, transform.position, transform.rotation);
                // 注入上下文，供角色内部组件使用
                playerObject.BattleInit(roleId, this);
                _allCharacters.Add(playerObject);
                index++;
            }

            // 批量创建怪物角色（从配置+预制体）
            List<Transform> monsterTrans = new List<Transform>(BattlePoint.Instance.GetMonsterTransforms());
            var monsterDataDic = BinaryDataMgr.Instance.GetConfig<MonsterInfoContainer>(E_ConfigLoadType.Editor).dataDic;
            index = 0;
            foreach (int monsterId in monsterDataDic.Keys)
            {
                if (index == monsterTrans.Count)
                {
                    break;
                }

                Transform transform = monsterTrans[index];
                PlayerObject monsterObject = await ObjectBuilder.GetOrCreateInstance<PlayerObject>(E_AssetBundleType.Prefab, ResKeyCollection.TestMonster, transform.position, transform.rotation);
                // 注入上下文，供角色内部组件使用
                monsterObject.BattleInit(monsterId, this);
                _allCharacters.Add(monsterObject);
                index++;
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
