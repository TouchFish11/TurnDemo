using System.Collections.Generic;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Event;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.StateMeachine;
using HotUpdate.Game.Battle.Turn;
using UnityEngine;

namespace HotUpdate.Game.Battle.Context
{
    /// <summary>
    /// 战斗上下文
    /// </summary>
    public class BattleContext : IBattleContext
    {
        public List<IBattleEntityObject> AllBattleEntity { get; } = new();
        
        public List<IBattleEntityObject> SceneMonsterObjects { get; }  = new();
        
        public List<IBattleEntityObject> SceneRoleObjects { get; }  = new();

        public List<ICommand> BattleCommands { get; } = new();
        
        public BattleEventBus EventBus { get; private set; }
        
        public IBattleStateMachine BattleMachine { get; private set; }
        
        public ICommand CurrentCommand { get; set; }
        
        public IBattleEntityObject CurrentTurnOwner { get; private set; }
        
        public float ActionLine { get; set; }
        
        public int CurentBattlePointCount { get; private set; } = 3;
        
        public int MaxBattlePointCount { get; private set; } = 5;

        public void Init(BattleEventBus eventBus, BattleStateMachine battleStateMachine)
        {
            EventBus = eventBus;
            BattleMachine = battleStateMachine;
        }

        /// <summary>
        /// 设置持有当前回合的行动实体
        /// </summary>
        /// <param name="battleEntityObject">持有当前当前回合的实体对象，若死亡则为null</param>
        public void SetCurrentTurnOwner(IBattleEntityObject battleEntityObject)
        {
            CurrentTurnOwner = battleEntityObject;
            if(CurrentTurnOwner != null)
                EventBus.TriggerEvent(new SwitchEntityTurnEvent(this, battleEntityObject));
        }
        
        public void ConsumeSkillPoint(int cost)
        {
            CurentBattlePointCount = Mathf.Clamp(CurentBattlePointCount - cost, 0, MaxBattlePointCount);
            EventBus.TriggerEvent(new OnBattlePointCountChangedEvent(this, CurentBattlePointCount, MaxBattlePointCount));
        }

        public void ExpandSkillPoint(int cost)
        {
            MaxBattlePointCount = Mathf.Max(0, MaxBattlePointCount - cost);
            EventBus.TriggerEvent(new OnBattlePointCountChangedEvent(this, CurentBattlePointCount, MaxBattlePointCount));
        }

        public void CleanData()
        {
            // 清理所有实体
            AllBattleEntity.Clear();
            SceneMonsterObjects.Clear();
            SceneRoleObjects.Clear();
            BattleCommands.Clear();
            
            EventBus = null;
            BattleMachine = null;
            CurrentTurnOwner = null;
            CurrentCommand = null;
        }

        public IEnumerable<IBattleEntityObject> GetAliveEntitys()
        {
            foreach (var battleEntityObject in AllBattleEntity)
            {
                if (!battleEntityObject.IsDead)
                {
                    yield return battleEntityObject;
                }
            }
        }
        
        public IEnumerable<IBattleEntityObject> GetAliveMonsterEntitys()
        {
            foreach (var battleEntityObject in AllBattleEntity)
            {
                if (battleEntityObject is MonsterObject && !battleEntityObject.IsDead)
                {
                    yield return battleEntityObject;
                }
            }
        }

        public IEnumerable<IBattleEntityObject> GetAlivePlayerEntitys()
        {
            foreach (var battleEntityObject in AllBattleEntity)
            {
                if (battleEntityObject is PlayerObject && !battleEntityObject.IsDead)
                {
                    yield return battleEntityObject;
                }
            }
        }
    }
}
