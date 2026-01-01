using Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly List<IBattleEntityObject> _allBattleEntity = new List<IBattleEntityObject>();
        // 回合管理器（核心依赖）
        private readonly TurnController _turnManager;
        // 当前战机点数
        private int currentBattlePointCount;
        // 最大战技点数
        private int maxBattlePointCount = 5;

        public int CurentBattlePointCount => currentBattlePointCount;

        public int MaxBattlePointCount => maxBattlePointCount;

        public BattleContext()
        {
            eventBus = new BattleEventBus();
            // 注入自身（IBattleContext）
            _turnManager = new TurnController(this);
            currentBattlePointCount = maxBattlePointCount;
        }

        /// <summary>
        /// 战斗初始化（启动战斗时调用）
        /// </summary>
        public async Task InitBattle()
        {
            // 创建战斗对象
            await CreateBattleEntity();
            // 初始化回合管理器
            _turnManager.InitActions(_allBattleEntity);
        }

        public void ConsumeSkillPoint(int cost)
        {
            currentBattlePointCount = Mathf.Clamp(currentBattlePointCount - cost, default, maxBattlePointCount);
            eventBus.TriggerEvent(new OnBattlePointCountChangedEvent(this, currentBattlePointCount, maxBattlePointCount));
        }

        public void ExpandSkillPoint(int cost)
        {
            maxBattlePointCount = Mathf.Max(default, maxBattlePointCount - cost);
            eventBus.TriggerEvent(new OnBattlePointCountChangedEvent(this, currentBattlePointCount, maxBattlePointCount));
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
            var playerDataDic = BinaryDataManager.Instance.GetConfig<RoleInfoContainer>(E_ConfigLoadType.Editor).dataDic;
            int index = 0;
            foreach (int roleId in playerDataDic.Keys)
            {
                if (index == playerTrans.Count)
                {
                    break;
                }

                Transform transform = playerTrans[index];

                PlayerObject playerObject = await RoleBuilder.CreateRole(roleId, transform);
                // 注入上下文，供角色内部组件使用
                playerObject.BattleInit(roleId, this);
                _allBattleEntity.Add(playerObject);
                index++;
            }

            // 批量创建怪物角色（从配置+预制体）
            List<Transform> monsterTrans = new List<Transform>(BattlePoint.Instance.GetMonsterTransforms());
            var monsterDataDic = BinaryDataManager.Instance.GetConfig<MonsterInfoContainer>(E_ConfigLoadType.Editor).dataDic;
            index = 0;
            foreach (int monsterId in monsterDataDic.Keys)
            {
                if (index == monsterTrans.Count)
                {
                    break;
                }

                Transform transform = monsterTrans[index];
                MonsterObject monsterObject = await MonsterBuilder.CreateMonster(monsterId, transform);
                // 注入上下文，供角色内部组件使用
                monsterObject.BattleInit(monsterId, this);
                _allBattleEntity.Add(monsterObject);
                index++;
            }
        }

        /// <summary>
        /// 战斗结束清理（避免内存泄漏）
        /// </summary>
        public void CleanupBattle()
        {
            // 销毁所有角色 GameObject（Unity 资源清理）
            foreach (IBattleEntityObject entity in _allBattleEntity)
            {
                Object.Destroy(entity.GameObject);
            }
            _allBattleEntity.Clear();
        }

        public IEnumerable<IBattleEntityObject> GetAllBattleEntity()
        {
            return _allBattleEntity;
        }

        public IEnumerable<IBattleEntityObject> GetPlayerObjects()
        {
            List<IBattleEntityObject> playerBattleEntityObjects = new List<IBattleEntityObject>();
            foreach (IBattleEntityObject battleEntity in _allBattleEntity)
            {
                if (battleEntity is PlayerObject player)
                {
                    playerBattleEntityObjects.Add(player);
                }
            }
            return playerBattleEntityObjects;
        }

        public IEnumerable<IBattleEntityObject> GetMonsterObjects()
        {
            List<IBattleEntityObject> monsterBattleEntityObjects = new List<IBattleEntityObject>();
            foreach (IBattleEntityObject battleEntity in _allBattleEntity)
            {
                if (battleEntity is MonsterObject monster)
                {
                    monsterBattleEntityObjects.Add(monster);
                }
            }
            return monsterBattleEntityObjects;
        }

        public TurnController GetTurnManager()
        {
            return _turnManager;
        }

        public BattleEventBus GetEventBus()
        {
            return eventBus;
        }
    }
}
