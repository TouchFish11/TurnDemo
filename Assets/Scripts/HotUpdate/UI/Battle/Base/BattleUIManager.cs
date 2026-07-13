using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Log;
using Core.Mono;
using Core.UI;
using HotUpdate.Base.Service;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Operation;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Statuses;
using HotUpdate.Game.Battle.UI;
using HotUpdate.Game.Battle.Utility;
using HotUpdate.UI.Battle.ActionLine;
using HotUpdate.UI.Battle.FloatText;
using HotUpdate.UI.Battle.Status;
using UnityEngine;
using BattlePointUI = HotUpdate.UI.Battle.BattlePoint.BattlePointUI;
using Logger = Core.Log.Logger;
using Random = UnityEngine.Random;
using SkillKeyUI = HotUpdate.UI.Battle.SkillKey.SkillKeyUI;
using TaskUtility = Core.Tasks.TaskUtility;

namespace HotUpdate.UI.Battle.Base
{
    /// <summary>
    /// 战斗界面管理器
    /// 负责战斗过程中所有UI的创建、更新、显示/隐藏等核心逻辑
    /// </summary>
    public class BattleUIManager : IBattleUIManager
    {
        [Inject] private ObjectSpawner _objectSpawner;
        [Inject] private IBattleCameraManager _battleCameraManager;
        [Inject] private IUIManager _uiManager;
        [Inject] private IMonoAdapter _monoAdapter;
        [Inject] private IconService _iconService;
        [Inject] private IBattleManager _battleManager;
        
        #region 私有字段
        // 战斗界面视图层引用
        private BattleView _view;
        // 战斗控制器引用
        private BattleController  _controller;

        /// <summary>
        /// 文本X轴偏移范围（随机）
        /// 控制伤害飘字的横向显示位置
        /// </summary>
        private readonly Vector2 textXOffsetRange = new(-0.5f, 0.5f);

        /// <summary>
        /// 文本Y轴偏移范围（随机）
        /// 控制伤害飘字的纵向显示位置
        /// </summary>
        private readonly Vector2 textYOffsetRange = new(0.45f, 0.55f);

        /// <summary>
        /// 通用等待协程对象（0.5秒）
        /// 复用避免重复创建，提升性能
        /// </summary>
        private static readonly WaitForSeconds s_waitForSeconds0_5 = new(0.5f);

        /// <summary>
        /// 通用等待协程对象（2.5秒）
        /// 复用避免重复创建，提升性能
        /// </summary>
        private static readonly WaitForSeconds s_waitForSeconds2_5 = new(2.5f);
        #endregion

        /// <summary>
        /// 战斗界面管理器构造函数
        /// </summary>
        /// <param name="view">战斗视图层实例</param>
        /// <param name="battleController"></param>
        public BattleUIManager(BattleView view, BattleController battleController)
        {
            _view = view;
            _controller = battleController;
        }
        
        #region 战斗状态相关
        /// <summary>
        /// 显示战斗结束界面
        /// 包含协程逻辑，控制界面显示时长后触发退出战斗事件
        /// </summary>
        /// <param name="context">战斗上下文，用于触发退出战斗事件</param>
        public void ShowBattleOver(IBattleContext context)
        {
            _monoAdapter.StartCoroutine(ShowBattleOver_Cor());
            return;

            // 战斗结束界面显示协程
            IEnumerator ShowBattleOver_Cor()
            {
                // 激活战斗结束UI区域
                _view.BattleStateTipArea.gameObject.SetActive(true);
                // 设置文本
                _view.SetBattleStateTipAreaText(true);
                
                yield return s_waitForSeconds2_5;

                // 隐藏战斗结束UI区域
                _view.BattleStateTipArea.gameObject.SetActive(false);

                yield return s_waitForSeconds0_5;

                // 触发退出战斗事件
                _battleManager.QuitBattle(_controller.PanelId);
            }
        }

        /// <summary>
        /// 显示战斗开始界面
        /// </summary>
        public void ShowBattleStart()
        {
            _monoAdapter.StartCoroutine(ShowBattleStart_Cor());
            return;

            // 战斗开始界面显示协程
            IEnumerator ShowBattleStart_Cor()
            {
                // 激活战斗结束UI区域
                _view.BattleStateTipArea.gameObject.SetActive(true);
                // 设置文本
                _view.SetBattleStateTipAreaText(false);
                
                yield return new WaitForSeconds(1.5f);

                // 隐藏战斗结束UI区域
                _view.BattleStateTipArea.gameObject.SetActive(false);
            }
        }
        #endregion

        #region 通用提示/文本相关
        /// <summary>
        /// 显示战斗提示信息
        /// 异步创建提示UI并初始化文本内容
        /// </summary>
        /// <param name="msg">要显示的提示文本内容</param>
        public async void ShowBattleMessage(string msg)
        {
            // 从资源包异步加载战斗提示UI预制体
            var battleMessageUI= await _objectSpawner.SpawnAsync<BattleMessageUI>(AssetKeys.BattleMessageUI, _view.BattleMsgArea);
            // 初始化提示文本（红色字体）
            battleMessageUI.InitMessage(Color.red, msg, _monoAdapter);
            battleMessageUI.OnDurationOver += messageUI => _objectSpawner.Release(messageUI);
        }

        /// <summary>
        /// 显示伤害文本（飘字）
        /// 包含伤害文本位置计算、UI初始化、累计伤害更新逻辑
        /// </summary>
        /// <param name="damageResult">伤害结算结果数据</param>
        public async void ShowDamageText(DamageResult damageResult)
        {
            // 从资源包异步加载伤害文本UI预制体
            var damageTextUI = await _objectSpawner.SpawnAsync<DamageTextUI>(AssetKeys.DamageTextUI);
            // 获取伤害文本的显示偏移位置（随机偏移）
            var dmgTextOffset = GetDamageTextUIPos(damageResult.Target, textXOffsetRange, textYOffsetRange);

            dmgTextOffset = damageResult.Target switch
            {
                PlayerObject => new Vector3(0, dmgTextOffset.y, dmgTextOffset.z),
                _ => dmgTextOffset
            };

            // 将世界坐标转换为UI本地坐标并设置文本位置
            if (UIUtility.WorldToLocalPointInRectangle(
                    _battleCameraManager.CurrentActiveCamera, 
                    _uiManager.UICamera, 
                    _view.ViewObj.transform, 
                    damageTextUI.gameObject, 
                    damageResult.Target.GameObject.transform.position + dmgTextOffset))
            {
                // 初始化伤害文本（元素颜色、伤害类型文本、最终伤害值）
                damageTextUI.InitDamageText(((int)damageResult.ElementType).ToElementTypeColor(), 
                    GetDamgeTypeText(damageResult), 
                    damageResult.FinalDamage, _monoAdapter);
                
                damageTextUI.OnDurationOver += dmgUI => _objectSpawner.Release(dmgUI);
            }
            
            // 更新累计伤害UI
            UpdateCumulativeDamage(true, damageResult.FinalDamage);
        }

        /// <summary>
        /// 显示护盾文本（飘字）
        /// 包含护盾文本位置计算、UI初始化
        /// </summary>
        /// <param name="target">目标战斗实体</param>
        /// <param name="sheilAmount">护盾量</param>
        public async void ShowShieldText(IBattleEntityObject target, int sheilAmount)
        {
            try
            {
                // 从资源包异步加载护盾文本UI预制体
                var shieldTextUI = await _objectSpawner.SpawnAsync<ShieldTextUI>(AssetKeys.ShieldTextUI);
                // 获取护盾文本的显示偏移位置（随机偏移）
                var dmgTextOffset = GetDamageTextUIPos(target, textXOffsetRange, textYOffsetRange);
            
                // 角色创建护盾文本，x不偏移，避免随机到摄像机外，无法显示
                dmgTextOffset = target switch
                {
                    PlayerObject => new Vector3(0, dmgTextOffset.y, dmgTextOffset.z),
                    _ => dmgTextOffset
                };
                
                // 将世界坐标转换为UI本地坐标并设置文本位置
                if (UIUtility.WorldToLocalPointInRectangle(
                        _battleCameraManager.CurrentActiveCamera, 
                        _uiManager.UICamera, 
                        _view.ViewObj.transform, 
                        shieldTextUI.gameObject, 
                        target.SubGameObject.transform.position + dmgTextOffset))
                {
                    // 初始化护盾文本
                    shieldTextUI.InitshieldText(sheilAmount, _monoAdapter);
                    shieldTextUI.OnDurationOver += shieldTextUI => _objectSpawner.Release(shieldTextUI);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.Battle, $"{nameof(BattleUIManager)}.{nameof(ShowShieldText)}：{e.Message}");
            }
        }

        /// <summary>
        /// 显示治疗文本（飘字）
        /// 逻辑与伤害文本类似
        /// </summary>
        /// <param name="target">治疗目标战斗实体</param>
        /// <param name="healAmount">治疗量</param>
        public async void ShowHealText(IBattleEntityObject target, int healAmount)
        {
            // 从资源包异步加载治疗文本UI预制体
            var healTextUI = await _objectSpawner.SpawnAsync<HealTextUI>(AssetKeys.HealTextUI);
            // 获取治疗文本的显示偏移位置（随机偏移）
            var dmgTextOffset = GetDamageTextUIPos(target, textXOffsetRange, textYOffsetRange);
            
            // 角色创建治疗文本，x不偏移，避免随机到摄像机外，无法显示
            dmgTextOffset = target switch
            {
                PlayerObject => new Vector3(0, dmgTextOffset.y, dmgTextOffset.z),
                _ => dmgTextOffset
            };
            
            // 将世界坐标转换为UI本地坐标并设置文本位置
            if (UIUtility.WorldToLocalPointInRectangle(
                    _battleCameraManager.CurrentActiveCamera, 
                    _uiManager.UICamera, 
                    _view.ViewObj.transform, 
                    healTextUI.gameObject, 
                    target.GameObject.transform.position + dmgTextOffset))
            {
                // 初始化治疗文本
                healTextUI.InitHealText(healAmount, _monoAdapter);
                healTextUI.OnDurationOver += healUI => _objectSpawner.Release(healUI);
            }
        }
        
        /// <summary>
        /// 显示状态效果文本（Buff/Debuff飘字）
        /// 状态添加时显示对应的状态名称文本
        /// </summary>
        /// <param name="newStatus">新增的状态实例</param>
        public async void ShowStatusText(IStatus newStatus)
        {
            try
            {
                // 从资源包异步加载状态文本UI预制体
                var statusEffectTextUI = await _objectSpawner.SpawnAsync<StatusEffectTextUI>(AssetKeys.StatusEffectTextUI);
                // 计算状态文本显示位置
                if (UIUtility.WorldToLocalPointInRectangle(
                        _battleCameraManager.CurrentActiveCamera, 
                        DIContainer.GetInstance<IUIManager>().UICamera,
                        _view.BuffTextArea, statusEffectTextUI.gameObject, 
                        newStatus.Owner.SubGameObject.transform.position + Vector3.up * 0.5f, Vector2.zero))
                {
                    // 初始化状态文本（显示状态名称）
                    statusEffectTextUI.InitText(null, newStatus.StatusProperty.StatusInfo.f_name, _monoAdapter);
                    statusEffectTextUI.OnDurationOver += statusEffectTextUI => _objectSpawner.Release(statusEffectTextUI); 
                }
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.Battle, $"{nameof(BattleUIManager)}:{e.Message}");
            }
        }

        /// <summary>
        /// 更新累计伤害UI显示
        /// 控制累计伤害区域的激活状态，并更新数值
        /// </summary>
        /// <param name="isShow">是否显示累计伤害UI</param>
        /// <param name="dmg">本次新增伤害值</param>
        public void UpdateCumulativeDamage(bool isShow, int dmg)
        {
            // 设置累计伤害UI区域激活状态
            _view.TotalDmgArea.gameObject.SetActive(isShow);
            // 更新累计伤害数值
            _view.UpdateCumulativeTotalDmg(_view.SetCumulativeDamage(dmg, !isShow));
        }
        #endregion

        #region 行动队列/ActionBar相关
        
        public void SlidingActionGrids(IBattleContext context)
        {
            var displayEntities = context.AllBattleEntity.FindAll(be => be != context.CurrentTurnOwner);
            // 其它格子需要移动
            for (var i = 0; i < displayEntities.Count; i++)
            {
                var battleEntityObject = displayEntities[i];
                var grid = _view.ActionGridUis.Find(ui => ui.BattleEntity == battleEntityObject);
                if (grid)
                {
                    // 设置滑动到的目标索引
                    grid.SetSlideTarget(i);
                    // 设置行动值
                    grid.SetActionValue(CalcRemainActionValue(context, battleEntityObject.ActionValue));
                }
            }
        }
        
        public void RemoveActionGrid(IBattleEntityObject battleEntity)
        {
            var actionGridUI = _view.ActionGridUis.Find(ui => ui.BattleEntity == battleEntity);
            _view.ActionGridUis.Remove(actionGridUI);
            _objectSpawner.Release(actionGridUI);
        }
        
        public void SwitchTurnUpdateActionGrid(IBattleEntityObject battleEntity)
        {
            RemoveActionGrid(battleEntity);
            
            // 其它格子需要移动
            var context = battleEntity.Context;
            var list = new List<IBattleEntityObject>(context.GetAliveEntitys());
            list.Remove(battleEntity);
            
            for (var i = 0; i < list.Count; i++)
            {
                var battleEntityObject = list[i];
                var grid = _view.ActionGridUis.Find(ui => ui.BattleEntity == battleEntityObject);
                if (grid)
                {
                    // 设置滑动到的目标索引
                    grid.SetSlideTarget(i);
                    // 设置行动值
                    grid.SetActionValue(CalcRemainActionValue(context, battleEntityObject.ActionValue));
                }
            }
        }
        
        /// <summary>
        /// 设置当前执行指令的对象的Icon
        /// </summary>
        /// <param name="battleEntity"></param>
        public async void SetCurrentCommanderDisplayUI(IBattleEntityObject battleEntity)
        {
            try
            {
                // 不显示当前执行指令的对象图标
                if (battleEntity == null)
                {
                    _view.ActionExecuteGrid.UpdateGrid(null, null);
                    return;
                }
                
                // 获取实体对应的图标名称
                var icon = await GetIconByEntity(battleEntity);
                _view.ActionExecuteGrid.UpdateGrid(icon, battleEntity);
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.Battle, $"[{nameof(BattleUIManager)}]: {e.Message}");
            }
        }

        /// <summary>
        /// 更新等待行动队列UI内容
        /// 为每个等待行动的战斗实体创建对应的UI并初始化
        /// </summary>
        public async void UpdateWaitingContent(IBattleContext context, List<IDisplayPendingExecution> displayPendingExecutions)
        {
            try
            {
                _objectSpawner.Release(_view.WaitingActUIs);
                foreach (var displayPendingExecution in displayPendingExecutions)
                {
                    // 异步加载等待行动UI预制体
                    var waitingActUI = await _objectSpawner.SpawnAsync<WaitingActUI>(AssetKeys.WaitingActUI, _view.WaitQueueContent);
                    // 加载图标精灵并初始化UI
                    var icon = await GetIconByEntity(displayPendingExecution.BattleEntity);
                    // 初始化UI
                    waitingActUI.Init(icon, displayPendingExecution.BattleEntity.BattleEntityId);
                    // 缓存UI
                    _view.WaitingActUIs.Add(waitingActUI);
                }
            }
            catch (Exception e)
            {
                Logger.LogDebug(ELogTags.Battle, $"{e.Message}");
            }
        }

        /// <summary>
        /// 移除等待列表中的第一个UI
        /// </summary>
        public void RemoveFirstWaitingActUI()
        {
            if (_view.WaitingActUIs.Count > 0)
            {
                var waitingUI = _view.WaitingActUIs[0];
                _view.WaitingActUIs.RemoveAt(0);
                _objectSpawner.Release(waitingUI);
            }
        }

        public async Task InitActionbarContent(IBattleContext context)
        {
            // 特殊格子高度 + 间隙
            var startY = _view.ActionExecuteGrid.RectTransform.anchoredPosition.y - _view.ActionExecuteGrid.RectTransform.rect.height - 10;
            var startX = _view.ActionExecuteGrid.RectTransform.anchoredPosition.x;
            
            for (var i = 0; i < context.AllBattleEntity.Count; i++)
            {
                var battleEntity = context.AllBattleEntity[i];
                // 异步加载行动格子UI预制体
                var actionGridUI = await _objectSpawner.SpawnAsync<ActionGridUI>(AssetKeys.ActionGridUI, _view.ActionBarContent);
                // 加载图标精灵
                var icon = await GetIconByEntity(battleEntity);
                // 初始化行动格子UI：计算差值作为剩余行动值
                actionGridUI.Init(icon, startX, startY, i, battleEntity);
                // 设置行动值
                actionGridUI.SetActionValue(CalcRemainActionValue(context, battleEntity.ActionValue));
                _view.ActionGridUis.Add(actionGridUI);
            }
            
            Logger.LogDebug(ELogTags.Battle, $"Init actionbar finished");
        }
        
        public async void InsertActionGridToTarget(IBattleContext context)
        {
            try
            {
                var displayEntities = new List<IBattleEntityObject>(context.AllBattleEntity);
                
                // 特殊格子高度 + 间隙
                var startY = _view.ActionExecuteGrid.RectTransform.anchoredPosition.y - _view.ActionExecuteGrid.RectTransform.rect.height - 10;
                var startX = _view.ActionExecuteGrid.RectTransform.anchoredPosition.x;
                var girds = _view.ActionGridUis;
                
                for (var i = displayEntities.Count - 1; i >= 0; i--)
                {
                    var battleEntityObject = displayEntities[i];
                    var grid = girds.Find(ui => ui.BattleEntity == battleEntityObject);
                    // 存在直接更新格子位置
                    if (grid)
                    {
                        // 设置滑动到的目标索引
                        grid.SetSlideTarget(i);
                        // 设置行动值
                        grid.SetActionValue(CalcRemainActionValue(context, battleEntityObject.ActionValue));
                    }
                    // 新增格子
                    else
                    {
                        // 异步加载行动格子UI预制体
                        var actionGridUI = await _objectSpawner.SpawnAsync<ActionGridUI>(AssetKeys.ActionGridUI, _view.ActionBarContent);
                        // 加载图标精灵
                        var icon = await GetIconByEntity(battleEntityObject);
                        // 初始化行动格子UI：计算差值作为剩余行动值
                        actionGridUI.Init(icon, startX, startY, i, battleEntityObject);
                        // 设置行动值
                        actionGridUI.SetActionValue(CalcRemainActionValue(context, battleEntityObject.ActionValue));
                        girds.Add(actionGridUI);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.Battle, $"{typeof(BattleUIManager)}: Update action axis ui error,{e.Message}");
            }
        }

        /// <summary>
        /// 计算剩余行动值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        private static int CalcRemainActionValue(IBattleContext context, float currentValue)
        {
            var remainActionValue = (int)(currentValue - context.ActionLine);
            if (remainActionValue >= BattleUtility.MaxDisplayActionValue)
                remainActionValue = BattleUtility.MaxDisplayActionValue;
            return remainActionValue;
        }

        /// <summary>
        /// 设置行动格子高亮状态
        /// 根据选中的目标列表，高亮对应的行动格子
        /// </summary>
        /// <param name="selectedTargets">选中的目标实体列表</param>
        public void SetActionGridHighlights(List<IBattleEntityObject> selectedTargets)
        {
            // 获取模型层的行动格子UI列表
            var actionGridUI = _view.ActionGridUis;
            // 先清空所有格子的高亮状态
            foreach (var actionGrid in actionGridUI)
            {
                actionGrid.CheckSelect(null);
            }

            // 多目标选中：依次检查并高亮所有匹配的格子
            if (selectedTargets.Count > 1)
            {
                foreach (var actionGrid in actionGridUI)
                {
                    foreach (var battleEntity in selectedTargets)
                    {
                        if (!actionGrid.IsSelect)
                        {
                            actionGrid.CheckSelect(battleEntity);
                            _view.ActionExecuteGrid.CheckSelect(battleEntity);
                        }
                    }
                }
            }
            // 单目标选中：高亮匹配的格子
            else if (selectedTargets.Count == 1)
            {
                if (_view.ActionExecuteGrid.CheckSelect(selectedTargets[0]))
                {
                    return;
                }
                
                foreach (var actionGrid in actionGridUI)
                {
                    actionGrid.CheckSelect(selectedTargets[0]);
                }
            }
        }
        #endregion

        #region 技能操作/目标选择相关
        /// <summary>
        /// 清理选中目标标记UI
        /// 重置所有选中目标的视觉标记
        /// </summary>
        public void ClearSelectMarker()
        {
            _objectSpawner.Release(_view.SelectMarkerUIs);
            _view.SelectMarkerUIs.Clear();
        }

        /// <summary>
        /// 设置技能操作区UI
        /// 传入null则清空操作区
        /// </summary>
        public void ClearOperator()
        {
            // 清空操作区UI
            _objectSpawner.Release(_view.SkillKeyUIs);
            _view.SkillKeyUIs.Clear();
        }

        /// <summary>
        /// 设置行动提示的激活状态
        /// 控制"当前行动方"提示文本的显示/隐藏及内容
        /// </summary>
        /// <param name="actTipType">行动提示类型</param>
        public void SetActTipActive(EActTipType actTipType)
        {
            var isActive = actTipType != EActTipType.Hide;
            // 设置提示UI的激活状态
            _view.ActingTipUI.gameObject.SetActive(isActive);
            
            if (isActive)
            {
                // 更新提示文本（区分玩家/怪物行动）
                _view.ActingTipUI.UpdateTipText(actTipType == EActTipType.Monster);
            }
        }

        /// <summary>
        /// 更新技能操作区UI
        /// 根据当前行动实体和技能数据提供器，创建并初始化技能按键UI
        /// 需先调用SetActTipActive设置行动提示
        /// </summary>
        /// <param name="currentObject">当前行动的战斗实体</param>
        /// <param name="dataProvider">技能按键UI数据提供器</param>
        public async void UpdateOperator(IBattleEntityObject currentObject, ISkillKeyUIDataProvider dataProvider)
        {
            var skillKeyUIs = new List<SkillKeyUI>();
            // 获取当前实体的技能按键数据
            var skillKeyUIData = dataProvider.GetData(currentObject);
            var infos = skillKeyUIData.SkillInfos;
            
            foreach (var info in infos)
            {
                // 异步加载技能按键UI预制体
                var skillKeyUI = await _objectSpawner.SpawnAsync<SkillKeyUI>(AssetKeys.SkillKeyUI, _view.OperatorArea);
                // 初始化技能按键UI
                skillKeyUI.Init(info, _view.SkillKeyGroup, currentObject);
                skillKeyUIs.Add(skillKeyUI);
            }
            
            // 设置操作区UI列表
            _objectSpawner.Release(_view.SkillKeyUIs);
            _view.SkillKeyUIs.Clear();
            _view.SkillKeyUIs.AddRange(skillKeyUIs);
        }

        /// <summary>
        /// 设置目标标记UI
        /// 为选中的目标实体创建视觉标记，传入null则清空标记
        /// </summary>
        /// <param name="selectedTargets">选中的目标实体列表</param>
        /// <param name="skillTargetType"></param>
        public async void SetTargetMarkers(List<IBattleEntityObject> selectedTargets, E_SkillTargetType skillTargetType)
        {
            // 清空目标标记缓存
            _objectSpawner.Release(_view.SelectMarkerUIs);
            _view.SelectMarkerUIs.Clear();
            
            if (selectedTargets == null)
            {
                return;
            }
            
            foreach (var battleEntity in selectedTargets)
            {
                // 异步加载目标标记UI预制体
                var selectMarkerUI = await _objectSpawner.SpawnAsync<SelectMarkerUI>(AssetKeys.SelectMarkerUI);
                // 初始化目标标记
                selectMarkerUI.InitSelectMarker(battleEntity, skillTargetType, _view.SelectMarkerArea);
                // 缓存标记
                _view.SelectMarkerUIs.Add(selectMarkerUI);
            }
        }
        #endregion

        #region 战斗点数/状态条相关
        /// <summary>
        /// 更新战斗点数（能量/怒气）UI
        /// 动态创建点数UI并设置激活状态（已解锁/未解锁）
        /// </summary>
        /// <param name="current">当前可用点数</param>
        /// <param name="max">总点数上限</param>
        /// <returns>异步任务</returns>
        public async Task UpdateBattlePointCount(int current, int max)
        {
            var battlePointUIs = new List<BattlePointUI>();
            for (var i = 0; i < max; i++)
            {
                // 异步加载战斗点数UI预制体
                var battlePointUIWrapper = await _objectSpawner.SpawnAsync<BattlePointUI>( AssetKeys.BattlePointUI, _view.PointContent);
                // 设置点数激活状态（i < current 表示已解锁）
                battlePointUIWrapper.SetActivePoint(i < current);
                battlePointUIs.Add(battlePointUIWrapper);
            }
            
            _objectSpawner.Release(_view.BattlePointUIs);
            _view.BattlePointUIs.Clear();
            _view.BattlePointUIs.AddRange(battlePointUIs);
            // 刷新文本数显示
            _view.UpdateBattlePointCount(current);
            
            Logger.LogDebug(ELogTags.Battle, $"Init battlePoint ui finished");
        }

        /// <summary>
        /// 更新玩家状态条UI
        /// 刷新指定实体的血量、能量等状态数值显示
        /// </summary>
        /// <param name="currentBattleEntity">需要更新的战斗实体</param>
        public void UpdatePlayerStatuebar(IBattleEntityObject currentBattleEntity)
        {
            // 获取该实体对应的状态UI
            var roleStateUI = _view.RoleStateUIs.Find(r => r.RoleId == currentBattleEntity.BattleEntityId);
            if (roleStateUI)
            {
                // 刷新状态数值
                roleStateUI.UpdateStatus();
            }
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 显示角色立绘（必杀技）提示
        /// 短暂显示立绘图标和名称后隐藏
        /// </summary>
        /// <param name="roleInfo">角色信息</param>
        /// <param name="skillInfo">技能信息</param>
        public IEnumerator ShowPaiting(RoleInfo roleInfo, SkillInfo skillInfo)
        {
            // 加载角色立绘图标

            var iconTask = _iconService.LoadIconAsync(roleInfo.f_icon);
            yield return TaskUtility.WaitForTask(iconTask);
            
            // 启动协程控制显示时长
            yield return ShowPaiting_Cor(iconTask.Result, skillInfo);
            
            yield break;

            // 立绘显示协程
            IEnumerator ShowPaiting_Cor(Sprite icon, SkillInfo skillInfo)
            {
                // 显示立绘提示
                _view.UpdateUltimateShow(true, icon, skillInfo.f_name);
                // 显示1秒
                yield return new WaitForSeconds(1f);
                // 隐藏立绘提示
                _view.UpdateUltimateShow(false, null, string.Empty);
            }
        }

        /// <summary>
        /// 通过战斗实体获取对应的图标名称
        /// 区分玩家/怪物类型，返回不同的图标配置
        /// </summary>
        /// <param name="battleEntity">战斗实体</param>
        /// <returns>图标名称（用于加载精灵）</returns>
        public Task<Sprite> GetIconByEntity(IBattleEntityObject battleEntity)
        {
            var iconName = string.Empty;
            switch (battleEntity)
            {
                case PlayerObject playerObject:
                    // 玩家实体：使用角色配置的图标
                    iconName = playerObject.RoleInfo.f_icon;
                    break;
                case MonsterObject monsterObject:
                    // 怪物实体：使用怪物配置的图标
                    iconName = monsterObject.MonsterInfo.f_icon;
                    break;
                default:
                    // 未实现的实体类型：输出日志警告
                    Logger.LogDebug(ELogTags.Battle, $"未实现该类型实体的图标获取逻辑：{battleEntity}");
                    break;
            }
            
            return _iconService.LoadIconAsync(iconName);
        }

        /// <summary>
        /// 获取伤害文本的显示偏移位置
        /// 根据目标类型（玩家/怪物）的基础偏移 + 随机偏移计算最终位置
        /// </summary>
        /// <param name="dmgTarget">伤害目标实体</param>
        /// <param name="damageTextXOffsetRange">X轴随机偏移范围</param>
        /// <param name="damageTextYOffsetRange">Y轴随机偏移范围</param>
        /// <returns>伤害文本的最终偏移位置</returns>
        public static Vector3 GetDamageTextUIPos(IBattleEntityObject dmgTarget, Vector2 damageTextXOffsetRange, Vector2 damageTextYOffsetRange)
        {
            // 生成随机偏移值
            var x = Random.Range(damageTextXOffsetRange.x, damageTextXOffsetRange.y);
            var y = Random.Range(damageTextYOffsetRange.x, damageTextYOffsetRange.y);
            var dmgTextOffset = new Vector2(x, y);

            // 根据目标类型计算基础偏移
            var pos = dmgTarget switch
            {
                MonsterObject or PlayerObject => Vector3.one * dmgTextOffset,
                _ => default
            };

            return pos;
        }

        /// <summary>
        /// 获取伤害类型对应的文本描述
        /// 区分直接伤害、真实伤害、破甲伤害等类型
        /// </summary>
        /// <param name="result">伤害结算结果数据</param>
        /// <returns>伤害类型文本（如"暴击"、"真实"、"持续伤害"）</returns>
        public static string GetDamgeTypeText(DamageResult result)
        {
            var dmgTypeText = string.Empty;
            if (result.DamageType == E_DamageType.Direct)
            {
                // 直接伤害：区分暴击/普通
                dmgTypeText = result.IsCrit ? "暴击" : "";
            }
            else
            {
                // 特殊伤害类型：匹配对应的文本描述
                dmgTypeText = result.DamageType switch
                {
                    E_DamageType.True => "真伤",
                    E_DamageType.Break => "击破",
                    E_DamageType.SuperBreak => "超击破",
                    E_DamageType.Dot => "Dot",
                    _ => dmgTypeText
                };
            }

            return dmgTypeText;
        }

        /// <summary>
        /// 获取治疗文本的前缀
        /// 固定返回"+"，用于区分治疗与伤害
        /// </summary>
        /// <returns>治疗文本前缀（"+"）</returns>
        public static string GetHealText()
        {
            return "+";
        }
        #endregion

        public void Dispose()
        {
            _objectSpawner.Release(_view.ActionGridUis);
            _objectSpawner.Dispose();
            _iconService.Dispose();
            _objectSpawner = null;
            _iconService = null;
            _battleCameraManager = null;
            _uiManager = null;
            _view = null;
            _controller = null;
        }
    }
}