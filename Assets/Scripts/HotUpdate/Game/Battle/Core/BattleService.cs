using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DI;
using Core.Serialize.Binary;
using Core.Utility;
using HotUpdate.Base.Manager;
using HotUpdate.Base.UI;
using HotUpdate.Common.Config.ExcelInfo.Container;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Battle.UI;
using HotUpdate.Game.Battle.Utility;
using UnityEngine;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 战斗服务
    /// </summary>
    public class BattleService
    {
        [Inject] private IBinaryDataManager _binaryDataManager;
        [Inject] private IRoleFactory _roleFactory;
        [Inject] private IMonsterFactory _monsterFactory;
        [Inject] private BattlePointProxy _battlePointProxy;
        [Inject] private ICastSkillConditionFactory _castSkillConditionFactory;
        [Inject] private ITargetSelectStrategyFactory _targetSelectStrategyFactory;
        [Inject] private IBattleCameraManager _battleCameraManager;
        [Inject] private IUIService _uiService;
        
        private readonly IBattleManager _battleManager;
        private readonly IBattleContext _context;
        private readonly Commandfactory _commandFactory;

        public BattleService(IBattleManager battleManager, IBattleContext context)
        {
            _battleManager = battleManager;
            _context = context;
            _commandFactory = DIContainer.Create<Commandfactory>();
        }

        /// <summary>
        /// 创建并缓存玩家角色
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public async Task CreatePlayerRoles(params int[] roleIds)
        {
            var roles = new List<IBattleEntityObject>(roleIds.Length);
            var playerTrans = new List<Transform>(_battlePointProxy.BattlePoint.GetRoleTransforms());
            for (var i = 0; i < roleIds.Length; i++)
            {
                var handler = DIContainer.Create<RoleDeathHandler>();
                var roleId = roleIds[i];
                var roleInfo = _binaryDataManager.GetConfig<RoleInfoContainer>(EConfigLoadType.Excel).dataDic[roleId];
                var transform = playerTrans[i];
                // 创建角色对象
                var playerObject = await _roleFactory.CreateRole(roleId, transform);
                // 注入上下文，供角色内部组件使用
                playerObject.RoleBattleInit(new RoleBattleInitData
                {
                    RoleInfo = roleInfo,
                    BattleEntityId = roleId,
                    BattleContext = _context,
                    Commandfactory = _commandFactory,
                    CastSkillConditionFactory = _castSkillConditionFactory,
                    TargetSelectStrategyFactory = _targetSelectStrategyFactory,
                    DeathHandler = handler
                });
                // 记录角色所在的位置索引
                playerObject.EntityPosIndex = i;
                // 设置角色层级
                LayerUtility.SetLayerRecursively(playerObject.GameObject, LayerGeter.GetRoleLayerByIndex(i));
                roles.Add(playerObject);
            }
            
            // 缓存角色
            foreach (var battleEntityObject in roles)
            {
                _context.AddBattleEntity(battleEntityObject);
                _context.AddSceneRole(battleEntityObject);
            }
        }
        
        /// <summary>
        /// 创建并缓存怪物
        /// </summary>
        /// <returns></returns>
        public async Task<List<IBattleEntityObject>> CreateMonsters(int[] monsterIds)
        {
            var monsters = new List<IBattleEntityObject>(monsterIds.Length);
            var monsterTrans = new List<Transform>(_battlePointProxy.BattlePoint.GetMonsterTransforms());
            // 批量创建怪物
            if (monsterIds.Length == monsterTrans.Count)
            {
                for (var i = 0; i < monsterIds.Length; i++)
                {
                    var handle = DIContainer.Create<MonsterDeathHandler>();
                    var monsterId = monsterIds[i];
                    var monsterInfo = _binaryDataManager.GetConfig<MonsterInfoContainer>(EConfigLoadType.Excel).dataDic[monsterId];
                    var transform = monsterTrans[i];
                    // 创建怪物对象
                    var monsterObject = await _monsterFactory.CreateMonster(monsterId, transform);
                    // 设置名称
                    monsterObject.GameObject.name = $"{monsterObject.GameObject.name}_{i}";
                    // 注入上下文，供角色内部组件使用
                    monsterObject.MonsterBattleInit(new MonsterBattleInitData
                    {
                        MonsterInfo = monsterInfo,
                        BattleEntityId = monsterId,
                        BattleContext = _context,
                        Commandfactory = _commandFactory,
                        CastSkillConditionFactory = _castSkillConditionFactory,
                        TargetSelectStrategyFactory = _targetSelectStrategyFactory,
                        DeathHandler = handle
                    });
                    // 记录怪物所在的位置索引
                    monsterObject.EntityPosIndex = i;
                    // 设置怪物层级
                    LayerUtility.SetLayerRecursively(monsterObject.GameObject, LayerGeter.GetMonsterLayerByIndex(i));
                    // 缓存对象
                    monsters.Add(monsterObject);
                }
            }
            else if (monsterIds.Length == 1)
            {
                var handle = DIContainer.Create<MonsterDeathHandler>();
                var monsterId = monsterIds[0];
                var monsterInfo = _binaryDataManager.GetConfig<MonsterInfoContainer>(EConfigLoadType.Excel).dataDic[monsterId];
                var transform = monsterTrans[2];
                // 创建怪物对象
                var monsterObject = await _monsterFactory.CreateMonster(monsterId, transform);
                // 设置名称
                monsterObject.GameObject.name = $"{monsterObject.GameObject.name}_{2}";
                // 注入上下文，供角色内部组件使用
                monsterObject.MonsterBattleInit(new MonsterBattleInitData
                {
                    MonsterInfo = monsterInfo,
                    BattleEntityId = monsterId,
                    BattleContext = _context,
                    Commandfactory = _commandFactory,
                    CastSkillConditionFactory = _castSkillConditionFactory,
                    TargetSelectStrategyFactory = _targetSelectStrategyFactory,
                    DeathHandler = handle
                });
                // 记录怪物所在的位置索引
                monsterObject.EntityPosIndex = 2;
                // 设置怪物层级
                LayerUtility.SetLayerRecursively(monsterObject.GameObject, LayerGeter.GetMonsterLayerByIndex(2));
                // 缓存对象
                monsters.Add(monsterObject);
            }
            else
            {
                for (var i = 0; i < monsterIds.Length; i++)
                {
                    var handle = DIContainer.Create<MonsterDeathHandler>();
                    var monsterId = monsterIds[i];
                    var transform = monsterTrans[i + 1];
                    // 创建怪物对象
                    var monsterObject = await _monsterFactory.CreateMonster(monsterId, transform);
                    // 设置名称
                    monsterObject.GameObject.name = $"{monsterObject.GameObject.name}_{i + 1}";
                    var monsterInfo = _binaryDataManager.GetConfig<MonsterInfoContainer>(EConfigLoadType.Excel).dataDic[monsterId];
                    // 注入上下文，供角色内部组件使用
                    monsterObject.MonsterBattleInit(new MonsterBattleInitData
                    {
                        MonsterInfo = monsterInfo,
                        BattleEntityId = monsterId,
                        BattleContext = _context,
                        Commandfactory = _commandFactory,
                        CastSkillConditionFactory = _castSkillConditionFactory,
                        TargetSelectStrategyFactory = _targetSelectStrategyFactory,
                        DeathHandler = handle
                    });
                    // 记录怪物所在的位置索引
                    monsterObject.EntityPosIndex = i + 1;
                    // 设置怪物层级
                    LayerUtility.SetLayerRecursively(monsterObject.GameObject, LayerGeter.GetMonsterLayerByIndex(i + 1));
                    // 缓存对象
                    monsters.Add(monsterObject);
                }
            }
            
            foreach (var battleEntityObject in monsters)
            {
                _context.AddBattleEntity(battleEntityObject);
                _context.AddSceneMonster(battleEntityObject);
            }
            
            return monsters;
        }
        
        /// <summary>
        /// 首次入场，转波次都需要这样的逻辑
        /// </summary>
        /// <returns></returns>
        public IEnumerator UpdateWave()
        {
            // 隐藏行动轴UI，角色UI不用处理
            // ...
            
            // 调整相机视角
            yield return TaskUtility.WaitForTask(_battleCameraManager.CreateCamera(null, new Vector3(0, 1, -3.5f), Quaternion.identity));
            
            // 显示波次提示
            // TODO：可拓展ShowBattleStart方法，显示当前是第几回合的文本
            var controller = _uiService.GetPanel(EUIPanelId.BattlePanel) as IBattleController;
            controller.BattleUiManager.ShowBattleStart();
            
            // 创建入场特效
            // ...

            // 创建怪物
            yield return TaskUtility.WaitForTask(_battleManager.GetWaveCreator().CreateWave());
            
            // 初始化行动顺序
            BattleUtility.InitOrder(_context);
            
            // 初始化怪物UI
            yield return TaskUtility.WaitForTask(controller.UiInitializer.InitMonsterUIs(_context.GetAliveMonsterEntitys()));
            // 隐藏怪物UI
            controller.MonsterStateUIManager.InActiveMonsterUIs();

            yield return new WaitForSeconds(1f);

            // 显示行动轴UI
            // ...
        }
    }
}
