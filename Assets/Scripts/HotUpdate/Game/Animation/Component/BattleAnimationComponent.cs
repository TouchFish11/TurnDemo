using HotUpdate.Base.Animation;
using HotUpdate.Base.Component;
using HotUpdate.Base.Enums;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Object.Role;
using UnityEngine;

namespace HotUpdate.Game.Animation.Component
{
    /// <summary>
    /// 战斗动画组件
    /// 负责处理战斗实体（玩家/怪物）的各类战斗相关动画状态切换，
    /// 监听技能选择、技能释放等战斗事件，并根据事件触发对应动画
    /// </summary>
    [ComponentId(typeof(BattleAnimationComponent))]
    [ComponentCore(typeof(BattleAnimationComponentCore))]
    public class BattleAnimationComponent : BattleComponent, IAnimationComponent
    {
        private BattleAnimationComponentCore _battleAnimationComponentCore;
        
        protected override void OnBattleInit()
        {
            _battleAnimationComponentCore = (BattleAnimationComponentCore)ComponentCore;
            // 注册技能选择事件监听
            //battleEntity.Context.EventBus.AddListener<SelectSkillEvent>(OnSelectSkillEvent);
            // 初始化默认动画类型：玩家默认预普通攻击动画，其他实体（怪物）默认无动画
            _battleAnimationComponentCore.CurrentAnimationType = BattleEntity is IPlayerObject ? E_AnimationType.PreNormalAttack : E_AnimationType.None;
        }
        
        /// <summary>
        /// 设置动画播放状态
        /// 根据指定的动画类型触发对应的Animator Trigger参数
        /// </summary>
        /// <param name="type">要切换的动画类型</param>
        public void SetAnimationState(int type)
        {
            _battleAnimationComponentCore.SetAnimationState(type);
        }

        public Animator GetAnimator()
        {
            return _battleAnimationComponentCore.GetAnimator();
        }

        public AnimationParameter GetParameter()
        {
            return _battleAnimationComponentCore.GetParameter();
        }

        public AnimatorStateInfo GetCurrentAnimatorStateInfo(string layerName)
        {
            return _battleAnimationComponentCore.GetCurrentAnimatorStateInfo(layerName);
        }

        /// <summary>
        /// 设置必杀技姿态（触发预必杀技攻击动画）
        /// 提供给外部调用的快捷方法
        /// </summary>
        public void SetUltimatePose()
        {
            SetAnimationState((int)E_AnimationType.PreUltimateAttack);
        }

        /// <summary>
        /// 重置动画类型为初始状态
        /// 玩家重置为预普通攻击，其他实体重置为无动画
        /// </summary>
        public void ResetAnimationType()
        {
            _battleAnimationComponentCore.CurrentAnimationType = BattleEntity is IPlayerObject ? E_AnimationType.PreNormalAttack : E_AnimationType.None;
        }

        // /// <summary>
        // /// 技能选择事件回调
        // /// 根据选中的技能类型切换对应前置动画
        // /// </summary>
        // /// <param name="selectSkillEvent">技能选择事件数据</param>
        // private void OnSelectSkillEvent(SelectSkillEvent selectSkillEvent)
        // {
        //     // 过滤条件：事件触发者不是当前绑定实体，或触发者是怪物 → 不处理
        //     // TODO：分为玩家/怪物战斗动画组件
        //     if (selectSkillEvent.Caster != BattleEntity || selectSkillEvent.Caster is IMonsterObject)
        //     {
        //         return;
        //     }
        //
        //     // 从配置表中获取选中技能的配置信息
        //     var skillInfo = DIContainer.GetInstance<IBinaryDataManager>().GetConfig<SkillInfoContainer>(EConfigLoadType.Excel).dataDic[selectSkillEvent.SkillId];
        //     // 根据技能类型切换前置动画
        //     switch ((E_SkillType)skillInfo.f_SkillType)
        //     {
        //         case E_SkillType.Monster: // 怪物技能 → 播放通用攻击动画
        //             SetAnimationState((int)E_AnimationType.Attack);
        //             break;
        //         case E_SkillType.NormalAttack: // 普通攻击 → 播放预普通攻击动画
        //             SetAnimationState((int)E_AnimationType.PreNormalAttack);
        //             break;
        //         case E_SkillType.CombatSkill: // 战斗技能 → 播放预战斗技能攻击动画
        //             SetAnimationState((int)E_AnimationType.PreBattleAttack);
        //             break;
        //         case E_SkillType.EnhancedNormalAttack: // 强化普通攻击 → 暂未处理
        //             break;
        //         case E_SkillType.EnhancedCombatSkill: // 强化战斗技能 → 暂未处理
        //             break;
        //     }
        // }
        
        protected override void OnBattleDestroy()
        {
            _battleAnimationComponentCore = null;
        }
    }
}