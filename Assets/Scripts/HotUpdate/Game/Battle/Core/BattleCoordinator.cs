using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DI;
using Core.Serialize.Binary;
using Core.Utility;
using HotUpdate.Base.Manager;
using HotUpdate.Base.UI;
using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Inputs;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Battle.UI;
using HotUpdate.Game.Battle.UI.Provider;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 战斗协调器
    /// </summary>
    public class BattleCoordinator : IDisposable
    {
        [Inject] private BattlePointProxy _battlePointProxy;
        [Inject] private ISkillKeyUIDataProviderFactory _skillKeyUIDataProviderFactory;
        [Inject] private IBinaryDataManager _binaryDataManager;
        [Inject] private IUIService _uiService;
        
        // 当前选中技能的配置信息
        private SkillInfo skillInfo;
        // 技能释放者（释放当前技能的战斗实体）
        private IBattleEntityObject caster;
        // 是否激活目标选择
        private bool _isActiveTargetSelect;
        // 是否激活战斗输入
        private bool _isActiveInput;
        
        public IBattleInputHandler BattleInputHandler { get; }
        public IBattleCameraManager BattleCameraManager { get; private set; }
        public ITargetSelectManager TargetSelectManager { get; private set; }
        public IBattleContext Context { get; private set; }

        /// <summary>
        /// 是否激活目标选择
        /// </summary>
        public bool IsActiveTargetSelect
        {
            get => _isActiveTargetSelect;
            set
            {
                // 激活目标选择
                if (value && !_isActiveTargetSelect)
                {
                    BattleInputHandler.OnLeftDrag += OnLeftDrag;   // 左拖拽：切换上一个主目标
                    BattleInputHandler.OnRightDrag += OnRightDrag;     // 右拖拽：切换下一个主目标
                    BattleInputHandler.OnClick += OnClick;
                }
                // 禁用目标选择
                else if(!value && _isActiveTargetSelect)
                {
                    BattleInputHandler.OnLeftDrag -= OnLeftDrag;
                    BattleInputHandler.OnRightDrag -= OnRightDrag;
                    BattleInputHandler.OnClick -= OnClick;
                }

                _isActiveTargetSelect = value;
            }
        }

        /// <summary>
        /// 是否激活战斗输入
        /// </summary>
        public bool IsActiveInput
        {
            get => _isActiveInput;
            set
            {
                if (value && !_isActiveInput)
                {
                    BattleInputHandler.SetInputState(true);
                }
                else if (!value && _isActiveInput)
                {
                    BattleInputHandler.SetInputState(false);
                }
            }
        }
        
        public BattleCoordinator(IBattleInputHandler battleInputHandler)
        {
            BattleInputHandler = battleInputHandler;
            IsActiveTargetSelect = true;
        }

        /// <summary>
        /// 初始化战斗协调器
        /// </summary>
        /// <param name="battleContext"></param>
        public void Init(IBattleContext battleContext)
        {
            BattleCameraManager = DIContainer.Create<IBattleCameraManager>(parameterValues: new object[] { this, BattleInputHandler });
            TargetSelectManager = DIContainer.Create<ITargetSelectManager>();
            // 初始化角色战斗点，依赖玩家战斗实体对象创建完成
            _battlePointProxy.InitProxy(battleContext, new List<IBattleEntityObject>(Context.GetAlivePlayerEntitys()));
            Context = battleContext;
        }
        
        /// <summary>
        /// 初始化技能目标
        /// </summary>
        /// <param name="skill"></param>
        public void InitSkillTarget(ISkill skill)
        {
            var mainTaget = TargetSelectManager.GetMainTarget();
            var selectedTargets = TargetSelectManager.GetTargets();
            skill.Init(mainTaget, selectedTargets);
        }

        /// <summary>
        /// 设置技能信息缓存
        /// </summary>
        /// <param name="skillInfo"></param>
        public void SetSelectSkillInfo(SkillInfo skillInfo)
        {
            this.skillInfo = skillInfo;
        }

        /// <summary>
        /// 根据技能信息自动重新计算主目标和范围内的目标，触发目标选择事件更新UI显示,在选择目标前要先设置SetSelectSkillInfo
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="targetSelectStrategy"></param>
        public void SelectTargets(IBattleEntityObject caster, ITargetSelectStrategy targetSelectStrategy)
        {
            // 自动重新计算主目标
            TargetSelectManager.SelectMainTarget(Context, caster, skillInfo, targetSelectStrategy);
            // 基于主目标更新范围目标列表
            TargetSelectManager.UpdateTargets(skillInfo.f_skillRangeType);
            // 触发目标选择变更事件，通知UI更新选中状态
            Context.GetEventBus().TriggerEvent(new SelectTargetEvent(Context, caster, TargetSelectManager.GetMainTarget(), TargetSelectManager.GetTargets()));
        }

        /// <summary>
        /// 设置相机的位置变换,若相机不存在则异步创建
        /// </summary>
        /// <param name="cameraTrans"></param>
        /// <param name="localPos"></param>
        /// <param name="localRot"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public Task SetCameraTrans(Transform cameraTrans, Vector3 localPos, Quaternion localRot, int mask)
        {
            return BattleCameraManager.CreateCamera(cameraTrans, localPos, localRot, mask);
        }
        
        /// <summary>
        /// 更新相机看向,看向怪物或玩家角色
        /// </summary>
        /// <param name="skillTargetType"></param>
        /// <param name="playerObject"></param>
        public async Task UpdateCamera(E_SkillTargetType skillTargetType, PlayerObject playerObject)
        {
            // 更新相机旋转基准
            BattleCameraManager.UpdateBaseRotation();
            
            switch (skillTargetType)
            {
                // 相机看向玩家角色
                case E_SkillTargetType.Friend:
                    // 失活所有怪物UI显示
                    ((IBattleController)_uiService.GetPanel(EUIPanelId.BattlePanel)).MonsterStateUIManager.InActiveMonsterUIs();
                    // TODO：更新相机看向玩家，计算相机世界坐标的位置和看向，数据暂时写死
                    var worldPos = new Vector3(0, 1, 1.7f);
                    var rotation = Quaternion.Euler(0, 180, 0);
                    // 获取遮罩
                    var mask = LayerGeter.GetRoleBitLayer() | LayerGeter.GetPreBitLayer();
                    // 创建相机
                    await BattleCameraManager.CreateCamera(null, worldPos, rotation, mask);
                    break;
                // 相机看向怪物目标
                case E_SkillTargetType.Enemy:
                    // 激活所有怪物UI显示
                    ((IBattleController)_uiService.GetPanel(EUIPanelId.BattlePanel)).MonsterStateUIManager.ActiveMonsterUIs();
                    // 更新对应的玩家相机看向怪物
                    var roleCameraParent = _battlePointProxy.BattlePoint.GetRoleCameraTransByIndex(playerObject.EntityPosIndex);
                    var mask2 = CalcRoleRenderMask(playerObject.EntityPosIndex);
                    await BattleCameraManager.CreateCamera(roleCameraParent, Vector3.zero, Quaternion.identity, mask2);
                    break;
                case E_SkillTargetType.None:
                default:
                    Logger.LogError($"{nameof(BattleCoordinator)}: Invalid target type,{skillTargetType}");
                    break;
            }
        }

        /// <summary>
        /// 执行玩家角色终结技释放前逻辑
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skillInfo"></param>
        public IEnumerator ExecutePreUltimateCast(IBattleEntityObject caster, SkillInfo skillInfo)
        {
            UpdateMonsterPos(caster);
            yield return TaskUtility.WaitForTask(UpdateCamera((PlayerObject)caster));
            // 玩家回合：激活目标选择功能
            IsActiveTargetSelect = true;
            // 启用输入
            IsActiveInput = true;

            var controller = (IBattleController)_uiService.GetPanel(EUIPanelId.BattlePanel);
            // 隐藏行动提示
            controller.BattleUiManager.SetActTipActive(E_ActTipType.Hide);
            // 激活怪物血量UI显示
            controller.MonsterStateUIManager.ActiveMonsterUIs();
            // 显示终结技立绘
            yield return controller.BattleUiManager.ShowPaiting(((PlayerObject)caster).RoleInfo, skillInfo);
            // 获取终结技技能按键UI数据提供器
            var provider = _skillKeyUIDataProviderFactory.GetProvider<UltimateSkillKeyUIDataProvider>();
            // 根据数据更新玩家操作按键，按键触发技能选择事件
            controller.BattleUiManager.UpdateOperator(caster, provider);
        }
        
        /// <summary>
        /// 根据释放技能的玩家角色，更新怪物在场景上的位置到预定的位置
        /// </summary>
        /// <param name="caster">释放技能的玩家角色对象</param>
        public void UpdateMonsterPos(IBattleEntityObject caster)
        {
            // 先执行战斗点位置变化
            _battlePointProxy.UpdateMonsterPos(caster);
        }
        
        /// <summary>
        /// 获取当前怪物中心点的位置
        /// </summary>
        /// <returns></returns>
        public Vector3 GetMonsterCenterPos()
        {
            return _battlePointProxy.BattlePoint.MonsterCenter.position;
        }

        /// <summary>
        /// 设置怪物中心点的位置为自定义的指定位置
        /// </summary>
        /// <param name="pos">自定义的指定位置</param>
        public void SetMonsterCenterPos(Vector3 pos)
        {
            _battlePointProxy.BattlePoint.MonsterCenter.position = pos;
        }

        public Vector3 GetRoleTransByIndex(int entityPosIndex)
        {
            return _battlePointProxy.GetRoleTransByIndex(entityPosIndex).position;
        }

        /// <summary>
        /// 传入玩家角色对象，更新相机的位置和渲染
        /// </summary>
        /// <param name="playerObject"></param>
        public async Task UpdateCamera(PlayerObject playerObject)
        {
            // 创建相机到指定位置点
            var roleCameraRoot = _battlePointProxy.GetRoleCameraRoot(playerObject);
            // 更新相机位置
            await BattleCameraManager.CreateCamera(roleCameraRoot, Vector3.zero, Quaternion.identity);
            // 更新相机渲染
            var mask = CalcRoleRenderMask(playerObject.EntityPosIndex);
            BattleCameraManager.CurrentActiveCamera.cullingMask = mask;
        }
        
        /// <summary>
        /// 基于指定角色的位置索引计算要渲染的玩家角色的Mask
        /// </summary>
        /// <param name="playerEntityPosIndex"></param>
        private static int CalcRoleRenderMask(int playerEntityPosIndex)
        {
            // 设置Mask
            var mask2 = ResetCameraMask();
            // 根据当前玩家位置索引，只渲染符合的角色
            var roleLayers = LayerGeter.GetRoleLayers();
            for (var i = playerEntityPosIndex; i < roleLayers.Length; i++)
            {
                mask2 |= 1 << roleLayers[i];
            }

            return mask2;
        }

        /// <summary>
        /// 重置相机Mask层级
        /// </summary>
        private static int ResetCameraMask()
        {
            var mask= LayerGeter.GetPreBitLayer();
            // TODO：暂时写所有怪物，后续优化
            mask |= LayerGeter.GetMonsterBitLayer();
            return mask;
        }
        
        private void OnLeftDrag()
        {
            TargetSelectManager.SelectPreviousMainTarget();
            TargetSelectManager.UpdateTargets(skillInfo.f_skillRangeType);
            // 触发目标选择变更事件，通知UI更新选中状态
            Context.GetEventBus().TriggerEvent(new SelectTargetEvent(Context, caster, TargetSelectManager.GetMainTarget(), TargetSelectManager.GetTargets()));
            // 更新相机选择基准
            BattleCameraManager.UpdateBaseRotation();
        }
        
        private void OnRightDrag()
        {
            TargetSelectManager.SelectNextMainTarget();
            TargetSelectManager.UpdateTargets(skillInfo.f_skillRangeType);
            // 触发目标选择变更事件，通知UI更新选中状态
            Context.GetEventBus().TriggerEvent(new SelectTargetEvent(Context, caster, TargetSelectManager.GetMainTarget(), TargetSelectManager.GetTargets()));
            // 更新相机选择基准
            BattleCameraManager.UpdateBaseRotation();
        }
        
        private void OnClick()
        {
            // 执行相机进行射线检测
            // 根据选中的技能ID获取技能配置信息
            // 将技能范围类型转换为技能目标类型（友方/敌方）
            var targetType = (E_SkillTargetType)skillInfo.f_SkillTargetType;

            // 根据技能目标类型设置射线检测的层级掩码（只检测对应层级的对象）
            int layerMask;
            switch (targetType)
            {
                case E_SkillTargetType.Friend:
                    // 检测玩家对象层级
                    layerMask = LayerGeter.GetRoleBitLayer();
                    break;
                case E_SkillTargetType.Enemy:
                    // 检测怪物对象层级
                    layerMask = LayerGeter.GetMonsterBitLayer();
                    break;
                case E_SkillTargetType.None:
                default:
                    Logger.LogWarning($"未处理的技能目标类型：{targetType}");
                    return;
            }
            
            var hitObj = BattleCameraManager.RayCast(layerMask);
            if (hitObj is BattleObject battleObject)
            {
                // 根据点击到的目标作为主目标
                TargetSelectManager.SelectMainTarget(battleObject);
                TargetSelectManager.UpdateTargets(skillInfo.f_skillRangeType);
                // 触发目标选择变更事件，通知UI更新选中状态
                Context.GetEventBus().TriggerEvent(new SelectTargetEvent(Context, caster, TargetSelectManager.GetMainTarget(), TargetSelectManager.GetTargets()));
            }
        }

        public void Dispose()
        {
            BattleInputHandler.OnLeftDrag -= OnLeftDrag;
            BattleInputHandler.OnRightDrag -= OnRightDrag;
        }
    }
}
