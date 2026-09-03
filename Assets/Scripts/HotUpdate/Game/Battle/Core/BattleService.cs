using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DI;
using Core.Log;
using Core.Mono;
using Core.Tasks;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Statuses;
using HotUpdate.Game.Battle.UI;
using HotUpdate.Game.Battle.Utility;
using HotUpdate.Game.VFX;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 战斗服务
    /// </summary>
    public class BattleService
    {
        [Inject] private IRoleFactory _roleFactory;
        [Inject] private IMonsterFactory _monsterFactory;
        [Inject] private BattlePointProxy _battlePointProxy;
        [Inject] private IBattleCameraManager _battleCameraManager;
        [Inject] private IUIService _uiService;
        [Inject] private IMonoAdapter _monoAdapter;
        [Inject] private IVFXManager _vfxManager;
        
        private IBattleManager _battleManager;
        private IBattleContext _context;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="battleManager"></param>
        /// <param name="context"></param>
        public void Init(IBattleManager battleManager, IBattleContext context)
        {
            _battleManager = battleManager;
            _context = context;
            Logger.LogDebug(ELogTags.Battle, $"BattleService init successfully");
        }

        /// <summary>
        /// 创建角色并缓存到战斗上下文中
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public async Task CreateRoles(params int[] roleIds)
        {
            for (var i = 0; i < roleIds.Length; i++)
            {
                // 创建角色对象
                var roleId = roleIds[i];
                var root = _battlePointProxy.BattlePoint.RoleTrans[i];
                var roleObject = await _roleFactory.CreateRole(roleId, i, _context, root); 
                // 缓存角色
                _context.AllBattleEntity.Add(roleObject);
                _context.SceneRoleObjects.Add(roleObject);
            }
            
            Logger.LogDebug(ELogTags.Battle, $"Role created successfully");
        }
        
        /// <summary>
        /// 创建怪物并缓存到战斗上下文中
        /// </summary>
        /// <returns></returns>
        public async Task<List<IBattleEntityObject>> CreateMonsters(int[] monsterIds)
        {
            var monsters = new List<IBattleEntityObject>(monsterIds.Length);
            // 批量创建怪物
            if (monsterIds.Length == _battlePointProxy.BattlePoint.MonsterTrans.Count)
            {
                for (var i = 0; i < monsterIds.Length; i++)
                {
                    var monsterId = monsterIds[i];
                    var transform = _battlePointProxy.BattlePoint.MonsterTrans[i];
                    var monsterObject = await _monsterFactory.CreateMonster(monsterId, i, _context, transform);
                    monsters.Add(monsterObject);
                }
            }
            else if (monsterIds.Length == 1)
            {
                var monsterId = monsterIds[0];
                var transform = _battlePointProxy.BattlePoint.MonsterTrans[2];
                var monsterObject = await _monsterFactory.CreateMonster(monsterId, 2, _context, transform);
                monsters.Add(monsterObject);
            }
            else
            {
                for (var i = 0; i < monsterIds.Length; i++)
                {
                    var monsterId = monsterIds[i];
                    var transform = _battlePointProxy.BattlePoint.MonsterTrans[i + 1];
                    var monsterObject = await _monsterFactory.CreateMonster(monsterId, i + 1, _context, transform);
                    monsters.Add(monsterObject);
                }
            }
            
            foreach (var battleEntityObject in monsters)
            {
                _context.AllBattleEntity.Add(battleEntityObject);
                _context.SceneMonsterObjects.Add(battleEntityObject);
            }
            
            Logger.LogDebug(ELogTags.Battle, $"Monster created successfully");
            return monsters;
        }
        
        /// <summary>
        /// 首次入场，转波次都需要这样的逻辑
        /// </summary>
        /// <returns></returns>
        public async Task UpdateWave()
        {
            await UpdateWaveWithoutPerformance();
            var controller = (IBattleController)_uiService.GetPanel(EUIPanelId.BattlePanel);
            // TODO：可拓展ShowBattleStart方法，显示当前是第几回合的文本
            controller.BattleUiManager.ShowBattleStart();
            // 创建入场特效
            // ...
            await Task.Delay(1000);
            controller.BattleUiManager.SetActionBarActive(true);
        }

        public async Task UpdateWaveWithoutPerformance()
        {
            // 清理上一波次残留的特效
            _vfxManager.ClearActiveVFX();
            var controller = (IBattleController)_uiService.GetPanel(EUIPanelId.BattlePanel);
            controller.BattleUiManager.SetActionBarActive(false);
            // 调整相机视角，设置相机mask
            var mask = LayerGeter.GetPreBitLayer() | LayerGeter.GetMonsterBitLayer();
            await _battleCameraManager.CreateCamera(null, new Vector3(0, 1, -3.5f), Quaternion.identity, mask);
            // 创建怪物
            await _battleManager.WaveCreator.CreateWave();
            // 初始化行动顺序并更新行动轴内容
            BattleUtility.InitOrder(_context);
            await controller.BattleUiManager.InitActionbarContent(_context);
            BattleUtility.UpdateOrder(_context);
            // 初始化怪物UI
            await controller.UiInitializer.InitMonsterUIs(_context.GetAliveMonsterEntitys());
            // 隐藏怪物UI
            controller.MonsterStateUIManager.InActiveMonsterUIs();
        }
        
        /// <summary>
        /// 处理死亡的战斗实体
        /// </summary>
        public IEnumerator HandleDeadEntity()
        {
            var deadSnapshot = _context.AllBattleEntity.FindAll(battleEntity => battleEntity.IsDead);
            if(deadSnapshot.Count == 0)
                yield break;
            
            var cTask = new List<Task>();
            // 播放死亡动画
            foreach (var battleEntity in deadSnapshot)
            {
                // 从上下文中移除死亡实体
                _context.AllBattleEntity.Remove(battleEntity);
                switch (battleEntity)
                {
                    case MonsterObject:
                        _context.SceneMonsterObjects.Remove(battleEntity);
                        break;
                    case RoleObject:
                        _context.SceneRoleObjects.Remove(battleEntity);
                        break;
                }
                
                var coroutine = _monoAdapter.StartCoroutine(battleEntity.Die());
                cTask.Add(TaskUtility.WaitForCoroutine(coroutine, _monoAdapter));

                if (battleEntity == _context.CurrentTurnOwner)
                {
                    _context.SetCurrentTurnOwner(null);
                }
                
                Logger.LogDebug(ELogTags.Battle, $"entity dead: {battleEntity}, playing death animation");
            }

            if (deadSnapshot.Count > 0)
            {
                // 触发实体死亡事件
                _context.EventBus.TriggerEvent(new EntityDeadEvent(_context, deadSnapshot));
                // 等待所有死亡动画处理完成
                yield return TaskUtility.WaitForTask(Task.WhenAll(cTask));
            }
        }

        /// <summary>
        /// 检查当前波次是否结束
        /// </summary>
        /// <returns></returns>
        public bool CheckWaveOver()
        {
            // 每次执行完命令后，检查战斗是否结束
            return _battleManager.WaveCreator.CheckOver();
        }

        /// <summary>
        /// 是否有剩余波次
        /// </summary>
        /// <returns></returns>
        public bool HasRemainWave()
        {
            return _battleManager.WaveCreator.TryMoveWave();
        }
        
        /// <summary>
        /// 推进到下一波，需要先判断是否有剩余波次
        /// </summary>
        public IEnumerator MoveWave()
        {
            yield return TaskUtility.WaitForTask(_battleManager.BattleService.UpdateWave());
        }

        /// <summary>
        /// 播放DOT结算演出
        /// </summary>
        /// <param name="entityObject"></param>
        public async Task PlaySettlementDot(IBattleEntityObject entityObject)
        {
            var statusComponent = entityObject.GetComponent<StatusComponent>();
            var hasDot = StatusUtility.ContainDot(statusComponent.GetStatuses());
            if (hasDot)
            {
                // 隐藏所有怪物血量UI显示
                ((IBattleController)_uiService.GetPanel(EUIPanelId.BattlePanel)).MonsterStateUIManager.InActiveMonsterUIs();
                // 调整相机角度
                var entityPos = entityObject.GameObject.transform.position;
                entityPos = new Vector3(entityPos.x, 1, entityPos.z);
                var pos = entityPos + entityObject.GameObject.transform.forward * 4;
                var rotation = Quaternion.LookRotation(entityPos - pos);
                var mask = LayerGeter.GetPreBitLayer() | (1 << entityObject.GameObject.layer);
                await _battleCameraManager.CreateCamera(null, pos, rotation, mask);
                // 等待Dot显示完成
                await Task.Delay(1400);
            }
        }
    }
}
