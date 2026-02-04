using System;
using System.Collections.Generic;
using Core.Pool;
using Core.Service;
using Game.Battle;
using Game.Battle.Context;
using Game.Battle.Event;
using Game.Battle.Objects;
using Game.Battle.Turn;
using GameHotUpdate.Battle.Event;
using GameHotUpdate.Battle.Event.Turn;
using GameHotUpdate.Condition;
using GameHotUpdate.Objects;
using GameHotUpdate.Turn;
using UnityEngine;

namespace GameHotUpdate.Context
{
    /// <summary>
    /// 战斗上下文
    /// </summary>
    public class BattleContext : IBattleContext
    {
        // 战斗事件总线实例
        private BattleEventBus _eventBus;
        // 战斗点代理
        private readonly IBattlePointProxy _battlePointProxy;
        // 战斗实体列表
        private List<IBattleEntityObject> _allBattleEntity = new();
        
        // 场景怪物列表
        private readonly List<IBattleEntityObject> _monsterObjects = new();
        // 场景玩家列表
        private readonly List<IBattleEntityObject> _roleObjects = new();
        // 场景召唤物列表
        // ...
        
        // 回合管理器
        private ITurnController _turnManager;
        // 当前行动实体
        private IBattleEntityObject _currentEntity;

        /// 当前战机点数
        public int CurentBattlePointCount { get; private set; }

        /// 最大战技点数
        public int MaxBattlePointCount { get; private set; } = 5;

        public BattleContext(IBattlePointProxy battlePointProxy)
        {
            _eventBus = new BattleEventBus();
            _battlePointProxy = battlePointProxy;
            // 注入自身（IBattleContext）
            _turnManager = new TurnController(this, new AllMonsterDeadCondition(), new AllPlayerDeadCondition());
            // 更新起始战技点
            CurentBattlePointCount = 3;
        }
        
        public void AddSceneMonster(IBattleEntityObject battleEntity)
        {
            _monsterObjects.Add(battleEntity);
        }
        
        public void AddSceneRole(IBattleEntityObject battleEntity)
        {
            _roleObjects.Add(battleEntity);
        }
        
        public void RemoveSceneMonster(IBattleEntityObject battleEntity)
        {
            _monsterObjects.Remove(battleEntity);
        }
        
        public void RemoveSceneRole(IBattleEntityObject battleEntity)
        {
            _roleObjects.Remove(battleEntity);
        }

        public List<IBattleEntityObject> GetSceneMonsters()
        {
            return _monsterObjects;
        }

        public List<IBattleEntityObject> GetSceneRoles()
        {
            return _roleObjects;
        }
        

        public void ConsumeSkillPoint(int cost)
        {
            CurentBattlePointCount = Mathf.Clamp(CurentBattlePointCount - cost, 0, MaxBattlePointCount);
            _eventBus.TriggerEvent(new OnBattlePointCountChangedEvent(this, CurentBattlePointCount, MaxBattlePointCount));
        }

        public void ExpandSkillPoint(int cost)
        {
            MaxBattlePointCount = Mathf.Max(0, MaxBattlePointCount - cost);
            _eventBus.TriggerEvent(new OnBattlePointCountChangedEvent(this, CurentBattlePointCount, MaxBattlePointCount));
        }

        public void CleanupBattle()
        {
            // 销毁所有角色 GameObject
            foreach (var entity in _allBattleEntity)
            {
                UnityEngine.Object.Destroy(entity.GameObject);
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

        public void AddBattleEntity(IBattleEntityObject battleEntity)
        {
            _allBattleEntity.Add(battleEntity);
        }

        public void Insert(int index, IBattleEntityObject battleEntityObject)
        {
            _allBattleEntity.Insert(index, battleEntityObject);
        }

        public bool RemoveBattleEntity(IBattleEntityObject battleEntity)
        {
            return _allBattleEntity.Remove(battleEntity);
        }

        public void Sort(Comparison<IBattleEntityObject> comparison)
        {
            _allBattleEntity.Sort(comparison);
        }

        public IEnumerable<IBattleEntityObject> GetAliveEntitys()
        {
            foreach (var battleEntityObject in _allBattleEntity)
            {
                if (!battleEntityObject.IsDead)
                {
                    yield return battleEntityObject;
                }
            }
        }

        public IEnumerable<IBattleEntityObject> GetDeadEntitys()
        {
            foreach (var battleEntityObject in _allBattleEntity)
            {
                if (battleEntityObject.IsDead)
                {
                    yield return battleEntityObject;
                }
            }
        }
        
        public IEnumerable<IBattleEntityObject> GetAliveMonsterEntitys()
        {
            foreach (var battleEntityObject in _allBattleEntity)
            {
                if (battleEntityObject is MonsterObject && !battleEntityObject.IsDead)
                {
                    yield return battleEntityObject;
                }
            }
        }

        public IEnumerable<IBattleEntityObject> GetDeadMonsterEntitys()
        {
            foreach (var battleEntityObject in _allBattleEntity)
            {
                if (battleEntityObject is MonsterObject && battleEntityObject.IsDead)
                {
                    yield return battleEntityObject;
                }
            }
        }

        public IEnumerable<IBattleEntityObject> GetAlivePlayerEntitys()
        {
            foreach (var battleEntityObject in _allBattleEntity)
            {
                if (battleEntityObject is PlayerObject && !battleEntityObject.IsDead)
                {
                    yield return battleEntityObject;
                }
            }
        }
        
        public IEnumerable<IBattleEntityObject> GetDeadPlayerEntitys()
        {
            foreach (var battleEntityObject in _allBattleEntity)
            {
                if (battleEntityObject is PlayerObject && battleEntityObject.IsDead)
                {
                    yield return battleEntityObject;
                }
            }
        }

        public int GetEntityIndex(IBattleEntityObject battleEntity)
        {
            return _allBattleEntity.IndexOf(battleEntity);
        }
        
        public int GetPlayerEntityIndex(IBattleEntityObject battleEntity)
        {
            return _allBattleEntity.IndexOf(battleEntity);
        }

        public int GetMonsterEntityIndex(IBattleEntityObject battleEntity)
        {
            return _allBattleEntity.IndexOf(battleEntity);
        }

        public IBattleEntityObject GetNextEntity()
        {
            return _allBattleEntity[0];
        }

        public IBattleEntityObject GetCurrentEntity()
        {
            return _currentEntity;
        }

        public void SetCurrentEntity(IBattleEntityObject battleEntity)
        {
            _currentEntity = battleEntity;
        }

        public IBattleEntityObject GetFirstBattleEntity()
        {
            return _allBattleEntity[0];
        }

        public ITurnController GetTurnManager()
        {
            return _turnManager;
        }

        public IBattleEventBus GetEventBus()
        {
            return _eventBus;
        }
        
        public IBattlePointProxy GetProxy()
        {
            return _battlePointProxy;
        }
    }
}
