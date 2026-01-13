using Framework;
using System;

namespace Game.Battle
{
    /// <summary>
    /// 战斗动画组件
    /// </summary>
    [ComponentId(nameof(BattleAnimationComponent))]
    public class BattleAnimationComponent : AnimationComponent, IBattleComponent
    {
        public IBattleEntityObject BattleEntity { get; private set; }
        public override int LayerIndex { get; protected set; }
        protected override E_AnimationType CurrentAnimationType { get; set; }

        public override void Init(IEntityObject entityObject)
        {
            base.Init(entityObject);
            BattleInit(entityObject as IBattleEntityObject);
            LayerIndex = animator.GetLayerIndex("Battle Layer");
            animator.SetLayerWeight(LayerIndex, 1);
        }

        public virtual void BattleInit(IBattleEntityObject battleEntity)
        {
            BattleEntity = battleEntity;
            battleEntity.Context.GetEventBus().AddListener<SelectSkillEvent>(OnSelectSkillEvent);
            battleEntity.Context.GetEventBus().AddListener<SkillCastEvent>(OnSkillCastEvent);

            CurrentAnimationType = (battleEntity is PlayerObject) ? E_AnimationType.PreNormalAttack : E_AnimationType.None;

        }

        public override void SetAnimationState(E_AnimationType animationType)
        {
            if (CurrentAnimationType == animationType)
            {
                return;
            }

            switch (animationType)
            {
                case E_AnimationType.None:
                    break;
                case E_AnimationType.PreNormalAttack:
                    animator.SetTrigger(animationArg.PreNormalAttackTriggerHash);
                    break;
                case E_AnimationType.NormalAttack:
                    animator.SetTrigger(animationArg.NormalAtkTirggerHash);
                    break;
                case E_AnimationType.PreBattleAttack:
                    animator.SetTrigger(animationArg.PreBattleAttackTriggerHash);
                    break;
                case E_AnimationType.BattleAttack:
                    animator.SetTrigger(animationArg.BattleAtkTriggerHash);
                    break;
                case E_AnimationType.PreUltimateAttack:
                    animator.SetTrigger(animationArg.PreUltimateAttackTriggerHash);
                    break;
                case E_AnimationType.UltimateAttack:
                    animator.SetTrigger(animationArg.UltimateAtkTriggerHash);
                    break;
                case E_AnimationType.Hit:
                    animator.SetTrigger(animationArg.HitTriggerHash);
                    break;
                case E_AnimationType.Death:
                    animator.SetTrigger(animationArg.DeathTriggerHash);
                    break;
                case E_AnimationType.Rebirth:
                    animator.SetTrigger(animationArg.RebirthTriggerHash);
                    break;
                case E_AnimationType.Attack:
                    animator.SetTrigger(animationArg.AttackTirggerHash);
                    break;
                default:
                    break;
            }
            CurrentAnimationType = animationType;
        }

        /// <summary>
        /// 设置终结技姿势
        /// </summary>
        public void SetUltimatePose()
        {
            SetAnimationState(E_AnimationType.PreUltimateAttack);
        }

        /// <summary>
        /// 重置动画类型状态
        /// </summary>
        public void ResetAnimationType()
        {
            if (this.BattleEntity is PlayerObject)
            {
                CurrentAnimationType = E_AnimationType.PreNormalAttack;
            }
            else
            {
                CurrentAnimationType = E_AnimationType.None;
            }
        }

        /// <summary>
        /// 选择技能事件回调
        /// </summary>
        /// <param name="selectSkillEvent"></param>
        private void OnSelectSkillEvent(SelectSkillEvent selectSkillEvent)
        {
            // TODO：暂时这样处理怪物逻辑，怪物选择技能不需要改变动画播放
            if (selectSkillEvent.Caster != this.BattleEntity || selectSkillEvent.Caster is MonsterObject)
            {
                return;
            }

            // 根据技能信息获取动画类型
            SkillInfo skillInfo = BinaryDataManager.Instance.GetConfig<SkillInfoContainer>(E_ConfigLoadType.Editor).dataDic[selectSkillEvent.SkillId];
            switch ((E_SkillType)skillInfo.f_SkillType)
            {
                case E_SkillType.Monster:
                    SetAnimationState(E_AnimationType.Attack);
                    break;
                case E_SkillType.NormalAttack:
                    SetAnimationState(E_AnimationType.PreNormalAttack);
                    break;
                case E_SkillType.CombatSkill:
                    SetAnimationState(E_AnimationType.PreBattleAttack);
                    break;
                case E_SkillType.EnhancedNormalAttack:
                    break;
                case E_SkillType.EnhancedCombatSkill:
                    break;
            }
        }

        /// <summary>
        /// 技能释放事件回调
        /// </summary>
        /// <param name="skillCastEvent"></param>
        private void OnSkillCastEvent(SkillCastEvent skillCastEvent)
        {
            if (skillCastEvent.Skill.Caster != this.BattleEntity)
            {
                return;
            }

            SetAnimationState((E_AnimationType)skillCastEvent.Skill.SkillInfo.f_animationType);
        }

        public override void Destroy()
        {
            base.Destroy();
            BattleEntity = null;
        }
    }
}
