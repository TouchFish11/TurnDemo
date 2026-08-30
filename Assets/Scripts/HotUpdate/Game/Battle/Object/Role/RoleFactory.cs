using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Serialize.Binary;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.StatSystem;
using HotUpdate.Game.Battle.TargetSelect;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role
{
    /// <summary>
    /// 角色工厂
    /// </summary>
    public class RoleFactory : IRoleFactory
    {
        [Inject] private ObjectSpawner _obectSpawner;
        [Inject] private IBinaryDataManager _binaryDataManager;
        [Inject] private Commandfactory _commandFactory;
        [Inject] private ICastSkillConditionFactory _castSkillConditionFactory;
        [Inject] private ITargetSelectStrategyFactory _targetSelectStrategyFactory;
        
        public async Task<IRoleObject> CreateRole(int roleId, int entityIndex, IBattleContext context, Transform parent, bool stay = false)
        {
            IRoleObject roleObject = roleId switch
            {
                1 => await _obectSpawner.SpawnAsync<Warrior.Warrior>(AssetKeys.Prefab_Warrior, parent, worldSpace:stay),
                2 => await _obectSpawner.SpawnAsync<Wizard.Wizard>(AssetKeys.Prefab_Wizard, parent, worldSpace:stay),
                3 => await _obectSpawner.SpawnAsync<Priest.Priest>(AssetKeys.Prefab_Priest, parent, worldSpace:stay),
                _ => null
            };
            
            var roleInfo = _binaryDataManager.GetConfig<RoleInfoContainer>(EConfigLoadType.Excel).dataDic[roleId];
            // 注入上下文，供角色内部组件使用
            roleObject.RoleBattleInit(new BattleParameterObject
            {
                BattleInfo = roleInfo,
                ResourceStatType = EResourceStatType.Energy,
                BattleEntityId = roleId,
                BattleContext = context,
                Commandfactory = _commandFactory,
                CastSkillConditionFactory = _castSkillConditionFactory,
                TargetSelectStrategyFactory = _targetSelectStrategyFactory,
                DeathHandler = DIContainer.Create<RoleDeathHandler>(),
            });
            
            // 记录角色所在的场景位置索引
            roleObject.EntityPosIndex = entityIndex;
            // 设置角色层级
            LayerUtility.SetLayerRecursively(roleObject.GameObject, LayerGeter.GetRoleLayerByIndex(entityIndex));
            // 初始化实体
            EntityHelper.InitEntity(roleObject);
            return roleObject;
        }
    }
}
