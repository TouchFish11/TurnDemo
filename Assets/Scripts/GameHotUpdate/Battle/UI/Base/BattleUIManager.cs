using System;
using System.Collections;
using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.Config;
using Core.Loader.Sprite;
using Core.Loader.UI;
using Core.Log;
using Core.Mono;
using Core.Service;
using Core.UI;
using Core.Utility;
using Game.Battle.Context;
using Game.Battle.Damage;
using Game.Battle.Objects;
using Game.Battle.Skill.Enum;
using Game.Battle.Status;
using Game.UI.Battle.SkillKey.Provider;
using GameHotUpdate.Battle.Event.Turn;
using GameHotUpdate.Battle.Object;
using GameHotUpdate.Battle.UI.ActionLine;
using GameHotUpdate.Battle.UI.FloatText;
using GameHotUpdate.Battle.UI.Status;
using GameHotUpdate.Cameras;
using GameHotUpdate.Tasks;
using UnityEngine;
using BattlePointUI = GameHotUpdate.Battle.UI.BattlePoint.BattlePointUI;
using Random = UnityEngine.Random;
using SkillKeyUI = GameHotUpdate.Battle.UI.SkillKey.SkillKeyUI;
using TaskUtility = Core.Utility.TaskUtility;

namespace GameHotUpdate.Battle.UI.Base
{
    /// <summary>
    /// 行动提示类型
    /// 用于控制战斗中"当前行动方"提示的显示状态
    /// </summary>
    public enum E_ActTipType : byte
    {
        /// <summary>
        /// 隐藏提示
        /// </summary>
        Hide,
        /// <summary>
        /// 玩家行动提示
        /// </summary>
        Player,
        /// <summary>
        /// 怪物行动提示
        /// </summary>
        Monster,
    }
    
    /// <summary>
    /// 战斗界面管理器
    /// 负责战斗过程中所有UI的创建、更新、显示/隐藏等核心逻辑
    /// </summary>
    public class BattleUIManager
    {
        #region 私有字段
        // 战斗界面视图层引用
        private readonly BattleView _view;
        // 战斗数据模型层引用
        private readonly BattleModel _model;
        // 战斗控制器引用
        private readonly BattleController  _controller;

        /// <summary>
        /// 伤害文本X轴偏移范围（随机）
        /// 控制伤害飘字的横向显示位置
        /// </summary>
        private readonly Vector2 damageTextXOffsetRange = new(-0.5f, 0.5f);

        /// <summary>
        /// 伤害文本Y轴偏移范围（随机）
        /// 控制伤害飘字的纵向显示位置
        /// </summary>
        private readonly Vector2 damageTextYOffsetRange = new(0.4f, 0.6f);

        /// <summary>
        /// 通用等待协程对象（0.5秒）
        /// 复用避免重复创建，提升性能
        /// </summary>
        private static readonly WaitForSeconds _waitForSeconds0_5 = new(0.5f);

        /// <summary>
        /// 通用等待协程对象（2.5秒）
        /// 复用避免重复创建，提升性能
        /// </summary>
        private static readonly WaitForSeconds _waitForSeconds2_5 = new(2.5f);
        #endregion

        /// <summary>
        /// 战斗界面管理器构造函数
        /// </summary>
        /// <param name="view">战斗视图层实例</param>
        /// <param name="model">战斗数据模型实例</param>
        /// <param name="battleController"></param>
        public BattleUIManager(BattleView view, BattleModel model, BattleController battleController)
        {
            _view = view;
            _model = model;
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
            ServiceLocator.Get<IMonoAdapter>().StartCoroutine(ShowBattleOver_Cor());
            return;

            // 战斗结束界面显示协程
            IEnumerator ShowBattleOver_Cor()
            {
                // 激活战斗结束UI区域
                _view.BattleStateTipArea.gameObject.SetActive(true);
                // 设置文本
                _view.SetBattleStateTipAreaText(true);
                
                yield return _waitForSeconds2_5;

                // 隐藏战斗结束UI区域
                _view.BattleStateTipArea.gameObject.SetActive(false);

                yield return _waitForSeconds0_5;

                // 触发退出战斗事件
                context.GetEventBus().TriggerEvent(new QuitBattleEvent(context, _controller));
            }
        }

        /// <summary>
        /// 显示战斗开始界面
        /// </summary>
        public void ShowBattleStart()
        {
            ServiceLocator.Get<IMonoAdapter>().StartCoroutine(ShowBattleStart_Cor());
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
            var battleMessageUIWrapper= await ServiceLocator.Get<IUiLoader>().GetUIObject<BattleMessageUI>(EAssetBundleType.UI, ResKeyCollection.BattleMessageUI, _view.BattleMsgArea);
            // 初始化提示文本（红色字体）
            battleMessageUIWrapper.InitMessage(Color.red, msg);
        }

        /// <summary>
        /// 显示伤害文本（飘字）
        /// 包含伤害文本位置计算、UI初始化、累计伤害更新逻辑
        /// </summary>
        /// <param name="damageResult">伤害结算结果数据</param>
        public async void ShowDamageText(DamageResult damageResult)
        {
            // 从资源包异步加载伤害文本UI预制体
            var damageTextUI = await ServiceLocator.Get<IUiLoader>().GetUIObject<DamageTextUI>(EAssetBundleType.UI, ResKeyCollection.DamageTextUI, null);
            // 获取伤害文本的显示偏移位置（随机偏移）
            var dmgTextOffset = GetDamageTextUIPos(damageResult.Target, damageTextXOffsetRange, damageTextYOffsetRange);
            
            // 将世界坐标转换为UI本地坐标并设置文本位置
            if (UIUtility.WorldToLocalPointInRectangle(
                    ServiceLocator.Get<IBattleCameraManager>().CurrentActiveCamera, 
                    UIManager.Instance.UICamera, _view.ViewObj.transform, 
                    damageTextUI.gameObject, 
                    damageResult.Target.GameObject.transform.position + dmgTextOffset))
            {
                // 初始化伤害文本（元素颜色、伤害类型文本、最终伤害值）
                damageTextUI.InitDamageText(((int)damageResult.ElementType).ToElementTypeColor(), GetDamgeTypeText(damageResult), damageResult.FinalDamage);
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
            // 从资源包异步加载护盾文本UI预制体
            var shieldTextUI = await ServiceLocator.Get<IUiLoader>().GetUIObject<ShieldTextUI>(EAssetBundleType.UI, ResKeyCollection.DamageTextUI, null);
            // 获取护盾文本的显示偏移位置（随机偏移）
            var dmgTextOffset = GetDamageTextUIPos(target, damageTextXOffsetRange, damageTextYOffsetRange);
            
            // 将世界坐标转换为UI本地坐标并设置文本位置
            if (UIUtility.WorldToLocalPointInRectangle(
                    ServiceLocator.Get<IBattleCameraManager>().CurrentActiveCamera, 
                    UIManager.Instance.UICamera, 
                    _view.ViewObj.transform, 
                    shieldTextUI.gameObject, 
                    target.GameObject.transform.position + dmgTextOffset))
            {
                // 初始化护盾文本
                shieldTextUI.InitshieldText(sheilAmount);
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
            var healTextUI = await ServiceLocator.Get<IUiLoader>().GetUIObject<HealTextUI>(EAssetBundleType.UI, ResKeyCollection.HealTextUI, null);
            // 获取治疗文本的显示偏移位置（随机偏移）
            var dmgTextOffset = GetDamageTextUIPos(target, damageTextXOffsetRange, damageTextYOffsetRange);
            
            // 将世界坐标转换为UI本地坐标并设置文本位置
            if (UIUtility.WorldToLocalPointInRectangle(
                    ServiceLocator.Get<IBattleCameraManager>().CurrentActiveCamera, 
                    UIManager.Instance.UICamera, 
                    _view.ViewObj.transform, 
                    healTextUI.gameObject, 
                    target.GameObject.transform.position + dmgTextOffset))
            {
                // 初始化治疗文本
                healTextUI.InitHealText(healAmount);
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
                var statusEffectTextUI = await ServiceLocator.Get<IUiLoader>().GetUIObject<StatusEffectTextUI>(EAssetBundleType.UI, ResKeyCollection.StatusEffectTextUI, null);
                // 计算状态文本显示位置
                if (UIUtility.WorldToLocalPointInRectangle(
                        ServiceLocator.Get<IBattleCameraManager>().CurrentActiveCamera, 
                        ServiceLocator.Get<IUIManager>().UICamera,
                        _view.BuffTextArea, statusEffectTextUI.gameObject, 
                        newStatus.Owner.SubGameObject.transform.position + Vector3.up * 0.5f, Vector2.zero))
                {
                    // 初始化状态文本（显示状态名称）
                    statusEffectTextUI.InitText(null, newStatus.StatusProperty.StatusInfo.f_name);
                }
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(BattleUIManager)}.{nameof(ShowStatusText)}：{e.Message}");
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
            // 更新累计伤害数值（模型层计算+视图层刷新）
            _view.UpdateTotalDmg(_model.SetCumulativeDamage(dmg, !isShow));
        }
        #endregion

        #region 行动队列/ActionBar相关
        /// <summary>
        /// 更新等待行动队列UI
        /// 为每个等待行动的战斗实体创建对应的UI并初始化
        /// </summary>
        /// <param name="battleEntities">等待行动的战斗实体列表</param>
        public async void UpdateWaitingCommmand(List<IBattleEntityObject> battleEntities)
        {
            try
            {
                _model.ClearWaitingActUI();
                foreach (var battleEntity in battleEntities)
                {
                    // 异步加载等待行动UI预制体
                    var waitingActUIWrapper = await ServiceLocator.Get<IUiLoader>().GetUIObject<WaitingActUI>(EAssetBundleType.UI, ResKeyCollection.WaitingActUI, _view.WaitQueueContent);
                    // 获取实体对应的图标名称
                    var iconName = GetIconByEntity(battleEntity);
                    // 加载图标精灵并初始化UI
                    var icon = await ServiceLocator.Get<ISpriteLoader>().LoadSpriteAsync(ResKeyCollection.Atlas_Icon_BattleEntity, iconName);
                    // 初始化UI
                    waitingActUIWrapper.Init(icon);
                    // 更新模型层的等待队列UI数据
                    _model.CacheWaitingCommmand(waitingActUIWrapper);
                }
            }
            catch (Exception e)
            {
                LogManager.Log($"{nameof(BattleUIManager)}.{nameof(UpdateWaitingCommmand)}：{e.Message}");
            }
        }

        /// <summary>
        /// 更新行动条（ActionBar）UI
        /// 为每个战斗实体创建行动格子UI，第一个实体的格子会特殊放大
        /// </summary>
        /// <param name="battleEntities">需要显示在行动条的战斗实体列表</param>
        public async void UpdateActionBar(IEnumerable<IBattleEntityObject> battleEntities)
        {
            try
            {
                // 清空缓存
                _model.ClearActionBar();
            
                // 标记是否为第一个实体（需要放大显示）
                var isFirst = true;
                foreach (var battleEntity in battleEntities)
                {
                    // 异步加载行动格子UI预制体
                    var actionGridUI = await ServiceLocator.Get<IUiLoader>().GetUIObject<ActionGridUI>(EAssetBundleType.UI, ResKeyCollection.ActionGridUI, _view.ActionBarContent);
                    // 获取实体对应的图标名称
                    var iconName = GetIconByEntity(battleEntity);
                    // 加载图标精灵
                    var icon = await ServiceLocator.Get<ISpriteLoader>().LoadSpriteAsync(ResKeyCollection.Atlas_Icon_BattleEntity, iconName);
                    // 初始化行动格子UI（图标、行动值、实体引用、是否第一个）
                    actionGridUI.Init(icon, battleEntity.ActionValue, battleEntity, isFirst);
                    // 更新模型层的行动条UI数据
                    _model.UpdateAcitonbar(actionGridUI);
                    isFirst = false;
                }
            }
            catch (Exception e)
            {
                LogManager.LogError($"{typeof(BattleUIManager)}.{nameof(UpdateActionBar)}；{e.Message}");
            }
        }

        /// <summary>
        /// 设置行动格子高亮状态
        /// 根据选中的目标列表，高亮对应的行动格子
        /// </summary>
        /// <param name="selectedTargets">选中的目标实体列表</param>
        public void SetActionGridHighlights(List<IBattleEntityObject> selectedTargets)
        {
            // 获取模型层的行动格子UI列表
            var actionGridUI = _model.GetActionGridUIs();

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
                        }
                    }
                }
            }
            // 单目标选中：高亮匹配的格子
            else if (selectedTargets.Count == 1)
            {
                foreach (var actionGrid in actionGridUI)
                {
                    foreach (var battleEntity in selectedTargets)
                    {
                        actionGrid.CheckSelect(battleEntity);
                    }
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
            _model.ClearSelectMarkers();
        }

        /// <summary>
        /// 设置技能操作区UI
        /// 传入null则清空操作区
        /// </summary>
        /// <param name="skillKeyUIs">技能按键UI列表</param>
        public void SetOperator(List<SkillKeyUI> skillKeyUIs)
        {
            if (skillKeyUIs == null)
            {
                // 清空操作区UI
                _model.ClearOperator();
                return;
            }
            // 设置操作区UI列表
            _model.SetOperator(skillKeyUIs);
        }

        /// <summary>
        /// 设置行动提示的激活状态
        /// 控制"当前行动方"提示文本的显示/隐藏及内容
        /// </summary>
        /// <param name="actTipType">行动提示类型</param>
        public void SetActTipActive(E_ActTipType actTipType)
        {
            var isActive = actTipType != E_ActTipType.Hide;
            // 设置提示UI的激活状态
            _view.ActingTipUI.gameObject.SetActive(isActive);
            
            if (isActive)
            {
                // 更新提示文本（区分玩家/怪物行动）
                _view.ActingTipUI.UpdateTipText(actTipType == E_ActTipType.Monster);
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
                var skillKeyUI = await ServiceLocator.Get<IUiLoader>().GetUIObject<SkillKeyUI>(EAssetBundleType.UI, ResKeyCollection.SkillKeyUI, _view.OperatorArea);
                // 初始化技能按键UI
                skillKeyUI.Init(info, _view.SkillKeyGroup, currentObject);
                skillKeyUIs.Add(skillKeyUI);
            }
            
            // 设置技能操作区UI
            SetOperator(skillKeyUIs);
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
            _model.ClearSelectMarkers();
            
            if (selectedTargets == null)
            {
                return;
            }
            
            foreach (var battleEntity in selectedTargets)
            {
                // 异步加载目标标记UI预制体
                var selectMarkerUI = await ServiceLocator.Get<IUiLoader>().GetUIObject<SelectMarkerUI>(EAssetBundleType.UI, ResKeyCollection.SelectMarkerUI, null);
                // 初始化目标标记
                selectMarkerUI.InitSelectMarker(battleEntity, skillTargetType, _view.SelectMarkerArea);
                // 缓存标记
                _model.AddSelectMarker(selectMarkerUI);
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
        public async System.Threading.Tasks.Task UpdateBattlePointCount(int current, int max)
        {
            var battlePointUIs = new List<BattlePointUI>();
            for (var i = 0; i < max; i++)
            {
                // 异步加载战斗点数UI预制体
                var battlePointUIWrapper = await ServiceLocator.Get<IUiLoader>().GetUIObject<BattlePointUI>(EAssetBundleType.UI, ResKeyCollection.BattlePointUI, _view.PointContent);
                // 设置点数激活状态（i < current 表示已解锁）
                battlePointUIWrapper.SetActivePoint(i < current);
                battlePointUIs.Add(battlePointUIWrapper);
            }
            
            // 更新模型层的战斗点数数据
            _model.UpdateBattlePointCount(current, battlePointUIs);
            // 刷新视图层的点数显示
            _view.UpdateBattlePointCount(current);
        }

        /// <summary>
        /// 更新玩家状态条UI
        /// 刷新指定实体的血量、能量等状态数值显示
        /// </summary>
        /// <param name="currentBattleEntity">需要更新的战斗实体</param>
        public void UpdatePlayerStatuebar(IBattleEntityObject currentBattleEntity)
        {
            // 获取该实体对应的状态UI
            var roleStateUI = _model.GetRoleStateUIById(currentBattleEntity.BattleEntityId);
            if (roleStateUI != null)
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
            var iconTask = ServiceLocator.Get<ISpriteLoader>().LoadSpriteAsync(ResKeyCollection.Atlas_Icon_BattleEntity, roleInfo.f_icon);
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
        public static string GetIconByEntity(IBattleEntityObject battleEntity)
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
                    LogManager.Log($"未实现该类型实体的图标获取逻辑：{battleEntity}");
                    break;
            }
            
            return iconName;
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
    }
}