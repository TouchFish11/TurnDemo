using Core.Components;
using Core.Serialize.Binary;
using Core.Service;
using GameHotUpdate.Battle.Event.UI;
using GameHotUpdate.Battle.Object;
using GameHotUpdate.Battle.Skill.Enum;
using GameHotUpdate.Component;

namespace GameHotUpdate.Animation.Component
{
    /// <summary>
    /// 战斗动画组件
    /// 负责处理战斗实体（玩家/怪物）的各类战斗相关动画状态切换，
    /// 监听技能选择、技能释放等战斗事件，并根据事件触发对应动画
    /// </summary>
    [ComponentId(typeof(BattleAnimationComponent))]
    public class BattleAnimationComponent : AnimationComponent
    {
        /// <summary>
        /// 当前绑定的战斗实体对象（玩家/怪物）
        /// </summary>
        public IBattleEntityObject BattleEntity { get; private set; }

        /// <summary>
        /// 当前播放的动画类型
        /// </summary>
        protected override E_AnimationType CurrentAnimationType { get; set; }

        /// <summary>
        /// 组件初始化方法
        /// </summary>
        /// <param name="entityObject">实体对象（需实现IBattleEntityObject接口）</param>
        public override void Init(IEntityObject entityObject)
        {
            base.Init(entityObject);
            // 获取子物体上的动画控制器组件
            animatorComponent = EntityObject.GetComponentInChildren<AnimatorComponent>();
            // 初始化战斗相关数据
            BattleInit(entityObject as IBattleEntityObject);
        }

        /// <summary>
        /// 战斗相关初始化
        /// 绑定战斗实体、注册战斗事件监听
        /// </summary>
        /// <param name="battleEntity">战斗实体对象</param>
        public void BattleInit(IBattleEntityObject battleEntity)
        {
            BattleEntity = battleEntity;
            // 注册技能选择事件监听
            battleEntity.Context.GetEventBus().AddListener<SelectSkillEvent>(OnSelectSkillEvent);
            // 初始化默认动画类型：玩家默认预普通攻击动画，其他实体（怪物）默认无动画
            CurrentAnimationType = (battleEntity is PlayerObject) ? E_AnimationType.PreNormalAttack : E_AnimationType.None;
        }

        /// <summary>
        /// 设置动画播放状态
        /// 根据指定的动画类型触发对应的Animator Trigger参数
        /// </summary>
        /// <param name="animationType">要切换的动画类型</param>
        public override void SetAnimationState(E_AnimationType animationType)
        {
            // 临时逻辑：若当前已在播放预普通攻击动画，且目标动画也是预普通攻击，则不重复触发
            if (animatorComponent.Animator.GetCurrentAnimatorStateInfo(animatorComponent.Animator.GetLayerIndex(Battle_Layer_Name)).IsName("PreNormalAttack") 
                && animationType == E_AnimationType.PreNormalAttack)
            {
                return;
            }

            // 根据动画类型触发对应的Animator Trigger
            switch (animationType)
            {
                case E_AnimationType.None: // 无动画
                    break;
                case E_AnimationType.PreNormalAttack: // 预普通攻击
                    animatorComponent.Animator.SetTrigger(animationArg.PreNormalAttackTriggerHash);
                    break;
                case E_AnimationType.NormalAttack: // 普通攻击
                    animatorComponent.Animator.SetTrigger(animationArg.NormalAtkTirggerHash);
                    break;
                case E_AnimationType.PreBattleAttack: // 预战斗技能攻击
                    animatorComponent.Animator.SetTrigger(animationArg.PreBattleAttackTriggerHash);
                    break;
                case E_AnimationType.BattleAttack: // 战斗技能攻击
                    animatorComponent.Animator.SetTrigger(animationArg.BattleAtkTriggerHash);
                    break;
                case E_AnimationType.PreUltimateAttack: // 预必杀技攻击
                    animatorComponent.Animator.SetTrigger(animationArg.PreUltimateAttackTriggerHash);
                    break;
                case E_AnimationType.UltimateAttack: // 必杀技攻击
                    animatorComponent.Animator.SetTrigger(animationArg.UltimateAtkTriggerHash);
                    break;
                case E_AnimationType.Hit: // 受击
                    animatorComponent.Animator.SetTrigger(animationArg.HitTriggerHash);
                    break;
                case E_AnimationType.Death: // 死亡
                    animatorComponent.Animator.SetTrigger(animationArg.DeathTriggerHash);
                    break;
                case E_AnimationType.Rebirth: // 复活
                    animatorComponent.Animator.SetTrigger(animationArg.RebirthTriggerHash);
                    break;
                case E_AnimationType.Attack: // 通用攻击（怪物默认）
                    animatorComponent.Animator.SetTrigger(animationArg.AttackTirggerHash);
                    break;
            }
            // 更新当前动画类型
            CurrentAnimationType = animationType;
        }

        /// <summary>
        /// 设置必杀技姿态（触发预必杀技攻击动画）
        /// 提供给外部调用的快捷方法
        /// </summary>
        public void SetUltimatePose()
        {
            SetAnimationState(E_AnimationType.PreUltimateAttack);
        }

        /// <summary>
        /// 重置动画类型为初始状态
        /// 玩家重置为预普通攻击，其他实体重置为无动画
        /// </summary>
        public void ResetAnimationType()
        {
            CurrentAnimationType = BattleEntity is PlayerObject ? E_AnimationType.PreNormalAttack : E_AnimationType.None;
        }

        /// <summary>
        /// 技能选择事件回调
        /// 根据选中的技能类型切换对应前置动画
        /// </summary>
        /// <param name="selectSkillEvent">技能选择事件数据</param>
        private void OnSelectSkillEvent(SelectSkillEvent selectSkillEvent)
        {
            // 过滤条件：事件触发者不是当前绑定实体，或触发者是怪物 → 不处理
            // TODO：分为玩家/怪物战斗动画组件
            if (selectSkillEvent.Caster != BattleEntity || selectSkillEvent.Caster is MonsterObject)
            {
                return;
            }

            // 从配置表中获取选中技能的配置信息
            var skillInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<SkillInfoContainer>(EConfigLoadType.Excel).dataDic[selectSkillEvent.SkillId];
            // 根据技能类型切换前置动画
            switch ((E_SkillType)skillInfo.f_SkillType)
            {
                case E_SkillType.Monster: // 怪物技能 → 播放通用攻击动画
                    SetAnimationState(E_AnimationType.Attack);
                    break;
                case E_SkillType.NormalAttack: // 普通攻击 → 播放预普通攻击动画
                    SetAnimationState(E_AnimationType.PreNormalAttack);
                    break;
                case E_SkillType.CombatSkill: // 战斗技能 → 播放预战斗技能攻击动画
                    SetAnimationState(E_AnimationType.PreBattleAttack);
                    break;
                case E_SkillType.EnhancedNormalAttack: // 强化普通攻击 → 暂未处理
                    break;
                case E_SkillType.EnhancedCombatSkill: // 强化战斗技能 → 暂未处理
                    break;
            }
        }

        /// <summary>
        /// 组件销毁方法
        /// 清理战斗实体引用，避免内存泄漏
        /// </summary>
        public override void Destroy()
        {
            base.Destroy();
            BattleEntity = null;
        }
    }
}