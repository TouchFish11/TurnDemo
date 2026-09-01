using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Serialize.Binary;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Object.Conditions;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.StatSystem;
using HotUpdate.Game.Battle.TargetSelect;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster
{
    /// <summary>
    /// 怪物工厂
    /// </summary>
    public class MonsterFactory : IMonsterFactory
    {
        [Inject] private ObjectSpawner _objectSpawner;
        [Inject] private IBinaryDataManager _binaryDataManager;
        [Inject] private Commandfactory _commandFactory;
        [Inject] private ICastSkillConditionFactory _castSkillConditionFactory;
        [Inject] private ITargetSelectStrategyFactory _targetSelectStrategyFactory;
        
        public async Task<IMonsterObject> CreateMonster(int monsterId, int entityIndex, IBattleContext context, Transform parent, bool stay = false)
        {
            IMonsterObject monsterObject = monsterId switch
            {
                1 => await _objectSpawner.SpawnAsync<Slime.Slime>(AssetKeys.Prefab_Slime, parent, worldSpace:stay),
                2 => await _objectSpawner.SpawnAsync<TurtleShell.TurtleShell>(AssetKeys.Prefab_TurtleShell, parent, worldSpace:stay),
                4 => await _objectSpawner.SpawnAsync<AbyssalMage.AbyssalMage>(AssetKeys.Prefab_AbyssalMage, parent, worldSpace:stay),
                _ => null
            };
            
            // 初始化实体
            EntityHelper.InitEntity(monsterObject);
            
            var monsterInfo = _binaryDataManager.GetConfig<MonsterInfoContainer>(EConfigLoadType.Excel).dataDic[monsterId];
            // 设置名称
            monsterObject.GameObject.name = $"{monsterObject.GameObject.name}_{entityIndex}";
            var deathHandler = DIContainer.Create<MonsterDeathHandler>();
            deathHandler.InitEntity(monsterObject);
            // 注入上下文，供角色内部组件使用
            monsterObject.MonsterBattleInit(new BattleParameterObject
            {
                BattleInfo = monsterInfo,
                BattleEntityId = monsterId,
                ResourceStatType = EResourceStatType.None,
                BattleContext = context,
                Commandfactory = _commandFactory,
                CastSkillConditionFactory = _castSkillConditionFactory,
                TargetSelectStrategyFactory = _targetSelectStrategyFactory,
                DeathHandler = deathHandler,
                DeathConditions = new List<IDeathCondition> { DIContainer.Create<ZeroHpCondition>() },
                TurnActionDriver = DIContainer.Create<MonsterAIDriver>(monsterObject)
            });
            
            // 记录怪物所在的位置索引
            monsterObject.EntityPosIndex = entityIndex;
            // 设置怪物层级
            LayerUtility.SetLayerRecursively(monsterObject.GameObject, LayerGeter.GetMonsterLayerByIndex(entityIndex));
            return monsterObject;
        }

        public void CollectDeadMonster(MonsterObject monsterObject)
        {
            _objectSpawner.Release(monsterObject);
        }
    }
}
