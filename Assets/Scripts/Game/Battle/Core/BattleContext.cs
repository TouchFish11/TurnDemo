using System.Collections.Generic;
using System.Threading.Tasks;
using Framework;
using UnityEngine;

namespace Game.Battle.Core
{
    /// <summary>
    /// 战斗上下文
    /// </summary>
    public class BattleContext : IBattleContext
    {
        // 战斗事件总线实例
        private BattleEventBus _eventBus;
        // 战斗实体列表
        private List<IBattleEntityObject> _allBattleEntity = new List<IBattleEntityObject>();
        // 回合管理器
        private TurnController _turnManager;
        // 当前行动实体
        private IBattleEntityObject _currentEntity;

        /// 当前战机点数
        public int CurentBattlePointCount { get; private set; }

        /// 最大战技点数
        public int MaxBattlePointCount { get; private set; } = 5;

        public BattleContext()
        {
            _eventBus = new BattleEventBus();
            // 注入自身（IBattleContext）
            _turnManager = new TurnController(this, new AllMonsterDeadCondition());
            CurentBattlePointCount = MaxBattlePointCount;
        }

        /// <summary>
        /// 战斗初始化
        /// 启动战斗时调用
        /// </summary>
        public async Task InitBattle()
        {
            // 创建战斗对象
            await CreateBattleEntity();
        }

        public void ConsumeSkillPoint(int cost)
        {
            CurentBattlePointCount = Mathf.Clamp(CurentBattlePointCount - cost, 0, MaxBattlePointCount);
            _eventBus.TriggerEvent(new OnBattlePointCountChangedEvent(this, CurentBattlePointCount, MaxBattlePointCount));
        }

        public void ExpandSkillPoint(int cost)
        {
            MaxBattlePointCount = Mathf.Max(default, MaxBattlePointCount - cost);
            _eventBus.TriggerEvent(new OnBattlePointCountChangedEvent(this, CurentBattlePointCount, MaxBattlePointCount));
        }

        /// <summary>
        /// 创建战斗实体对象
        /// </summary>
        /// <returns></returns>
        private async Task CreateBattleEntity(/*object config, int ownerId*/)
        {
            // TODO：可优化为使用战斗实体创建器来创建怪物、波次
            var playerTrans = new List<Transform>(BattlePoint.Instance.GetPlayerTransforms());
            // 批量创建玩家角色（从配置+预制体）
            var playerDataDic = BinaryDataManager.Instance.GetConfig<RoleInfoContainer>(E_ConfigLoadType.Editor).dataDic;
            var index = 0;
            foreach (var roleId in playerDataDic.Keys)
            {
                if (index == playerTrans.Count)
                {
                    break;
                }

                var transform = playerTrans[index];

                var playerObject = await RoleBuilder.CreateRole(roleId, transform);
                // 注入上下文，供角色内部组件使用
                playerObject.BattleInit(roleId, this);
                // 记录角色所在的位置索引
                playerObject.EntityPosIndex = index;
                _allBattleEntity.Add(playerObject);
                index++;
            }

            // 批量创建怪物角色（从配置+预制体）
            var monsterTrans = new List<Transform>(BattlePoint.Instance.GetMonsterTransforms());
            var monsterDataDic = BinaryDataManager.Instance.GetConfig<MonsterInfoContainer>(E_ConfigLoadType.Editor).dataDic;
            index = 0;
            foreach (int monsterId in monsterDataDic.Keys)
            {
                if (index == monsterTrans.Count)
                {
                    break;
                }

                var transform = monsterTrans[index];
                var monsterObject = await MonsterBuilder.CreateMonster(monsterId, transform);
                // 注入上下文，供角色内部组件使用
                monsterObject.BattleInit(monsterId, this);
                // 记录怪物所在的位置索引
                monsterObject.EntityPosIndex = index;
                _allBattleEntity.Add(monsterObject);
                index++;
            }
        }

        public void CleanupBattle()
        {
            // 销毁所有角色 GameObject
            foreach (var entity in _allBattleEntity)
            {
                Object.Destroy(entity.GameObject);
            }
            _allBattleEntity.Clear();
            _allBattleEntity = null;

            // 清空事件总线
            _eventBus.Clear();
            _eventBus = null;

            // 清空缓存池
            ServiceLocator.Get<IPoolManager>().Clear();

            _currentEntity = null;
            _turnManager = null;
        }

        public List<IBattleEntityObject> GetAllBattleEntity() => _allBattleEntity;

        public IBattleEntityObject GetFirstBattleEntity() => _allBattleEntity[0];

        public IEnumerable<IBattleEntityObject> GetLiveEntitys() => _allBattleEntity.FindAll((entity) => !entity.IsDead);

        public IEnumerable<IBattleEntityObject> GetDeadEntitys() => _allBattleEntity.FindAll((entity) => entity.IsDead);

        public List<IBattleEntityObject> GetDeadMonsterEntitys() => GetMonsterObjects().FindAll((monster) => monster.IsDead);

        public List<IBattleEntityObject> GetPlayerObjects() => _allBattleEntity.FindAll((entity) => entity is PlayerObject);

        public List<IBattleEntityObject> GetLivePlayerObjects() => GetPlayerObjects().FindAll((player) => !player.IsDead);

        public List<IBattleEntityObject> GetMonsterObjects() => _allBattleEntity.FindAll((entity) => entity is MonsterObject);

        public List<IBattleEntityObject> GetLiveMonsterObjects() => GetMonsterObjects().FindAll((monster) => !monster.IsDead);

        public int GetPlayerObjectIndex(IBattleEntityObject battleEntity) => GetPlayerObjects().IndexOf(battleEntity);

        public int GetMonsterObjectIndex(IBattleEntityObject battleEntity) => GetMonsterObjects().IndexOf(battleEntity);

        public IBattleEntityObject GetCurrentEntity() => _currentEntity;

        public void SetCurrentEntity(IBattleEntityObject battleEntity) => _currentEntity = battleEntity;

        public TurnController GetTurnManager() => _turnManager;

        public BattleEventBus GetEventBus() => _eventBus;
    }
}
