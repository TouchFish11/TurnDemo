using System.Collections;
using System.Threading.Tasks;
using Core.DI;
using Core.Log;
using Core.Serialize.Binary;
using Core.Tasks;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Operation;
using HotUpdate.Game.Battle.Operation.Provider;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Battle.UI;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 战斗协调器
    /// </summary>
    public class BattleCoordinator : IBattleCoordinator
    {
        [Inject] private ISkillKeyUIDataProviderFactory _skillKeyUIDataProviderFactory;
        [Inject] private IBinaryDataManager _binaryDataManager;
        [Inject] private IUIService _uiService;
        [Inject] private IBattlePointProxy _battlePointProxy;
        [Inject] private IBattleCameraManager _battleCameraManager;
        [Inject] private ITargetSelectManager _targetSelectManager;
        
        private readonly TargetSelectFlow _targetSelectFlow;
        
        public OperationState OperationState { get; private set; }
        
        public IBattleContext Context { get; private set; }

        private BattleCoordinator(TargetSelectFlow targetSelectFlow)
        {
            _targetSelectFlow = targetSelectFlow;
        }
        
        public void Init(IBattleContext battleContext, OperationState operationState)
        {
            _targetSelectFlow.Init(battleContext, operationState);
            Context = battleContext;
            OperationState = operationState;
        }
        
        public void InitSkillTarget(ISkill skill)
        {
            var mainTaget = _targetSelectManager.GetMainTarget();
            var selectedTargets = _targetSelectManager.GetTargets();
            skill.Init(mainTaget, selectedTargets);
        }
        
        public void SetSelectSkillInfo(SkillInfo skillInfo)
        {
            OperationState.CurrentSkillInfo = skillInfo;
        }
        
        public void SelectTargets(IBattleEntityObject caster, ITargetSelectStrategy targetSelectStrategy)
        {
            // 自动重新计算主目标
            _targetSelectManager.SelectMainTarget(Context, caster, OperationState.CurrentSkillInfo, targetSelectStrategy);
            // 基于主目标更新范围目标列表
            _targetSelectManager.SelectAllTargets(OperationState.CurrentSkillInfo.f_skillRangeType);
            OperationState.LastTarget = _targetSelectManager.GetMainTarget();
            OperationState.LastTargets = _targetSelectManager.GetTargets();
            // 怪物攻击：按攻击目标（角色）更新怪物整体位置，和该角色行动开始时一致
            if (caster is IMonsterObject && OperationState.LastTarget is IRoleObject targetRole)
            {
                _battlePointProxy.UpdateMonsterPos(targetRole);
            }
            // 触发目标选择变更事件，通知UI更新选中状态
            Context.EventBus.TriggerEvent(new SelectTargetEvent(Context, caster, _targetSelectManager.GetMainTarget(), _targetSelectManager.GetTargets()));
        }
        
        public async Task UpdateCamera(E_SkillTargetType skillTargetType, RoleObject roleObject)
        {
            // 更新相机旋转基准
            _battleCameraManager.UpdateBaseRotation();
            
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
                    await _battleCameraManager.CreateCamera(null, worldPos, rotation, mask);
                    break;
                // 相机看向怪物目标
                case E_SkillTargetType.Enemy:
                    // 激活所有怪物UI显示
                    ((IBattleController)_uiService.GetPanel(EUIPanelId.BattlePanel)).MonsterStateUIManager.ActiveMonsterUIs();
                    // 更新对应的玩家相机看向怪物
                    var roleCameraParent = _battlePointProxy.BattlePoint.RoleCamerasTrans[roleObject.EntityPosIndex];
                    var mask2 = _battleCameraManager.CalcRoleRenderMask(roleObject.EntityPosIndex);
                    await _battleCameraManager.CreateCamera(roleCameraParent, Vector3.zero, Quaternion.identity, mask2);
                    break;
                case E_SkillTargetType.None:
                default:
                    Logger.LogError(ELogTags.Battle, $"Invalid target type,{skillTargetType}");
                    break;
            }
        }
        
        public IEnumerator ExecutePreUltimateCast(IBattleEntityObject caster, SkillInfo skillInfo)
        {
            // 先执行战斗点位置变化
            _battlePointProxy.UpdateMonsterPos(caster);
            yield return TaskUtility.WaitForTask(UpdateCamera((RoleObject)caster));
            // 玩家回合：激活目标选择功能
            OperationState.IsActiveTargetSelect = true;
            // 启用输入
            OperationState.IsActiveInput = true;

            var controller = (IBattleController)_uiService.GetPanel(EUIPanelId.BattlePanel);
            // 隐藏行动提示
            controller.BattleUiManager.SetActTipActive(EActTipType.Hide);
            // 激活怪物血量UI显示
            controller.MonsterStateUIManager.ActiveMonsterUIs();
            // 显示终结技立绘
            yield return controller.BattleUiManager.ShowPaiting(((RoleObject)caster).RoleInfo, skillInfo);
            // 获取终结技技能按键UI数据提供器
            var provider = _skillKeyUIDataProviderFactory.GetProvider<UltimateSkillKeyUIDataProvider>();
            // 根据数据更新玩家操作按键，按键触发技能选择事件
            controller.BattleUiManager.UpdateOperator(caster, provider);
        }

        public async Task UpdateCamera(RoleObject roleObject)
        {
            // 创建相机到指定位置点
            var roleCameraRoot = _battlePointProxy.GetRoleCameraRoot(roleObject);
            // 更新相机位置
            await _battleCameraManager.CreateCamera(roleCameraRoot, Vector3.zero, Quaternion.identity);
            // 更新相机渲染
            var mask = _battleCameraManager.CalcRoleRenderMask(roleObject.EntityPosIndex);
            _battleCameraManager.CurrentActiveCamera.cullingMask = mask;
        }
        
        public void Reset()
        {
            _targetSelectFlow.Reset();
        }
    }
}
