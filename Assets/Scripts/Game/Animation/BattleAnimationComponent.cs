using Framework;

namespace Game.Battle
{
    /// <summary>
    /// 战斗动画组件
    /// </summary>
    public class BattleAnimationComponent : BaseAnimationComponent, IBattleComponent
    {
        public IBattleEntityObject BattleEntity { get; private set; }

        public override void Init(IEntityObject entityObject)
        {
            base.Init(entityObject);
            BattleInit(entityObject as IBattleEntityObject);
            animator.SetLayerWeight(animator.GetLayerIndex("Battle Layer"), 1);
        }

        public virtual void BattleInit(IBattleEntityObject battleEntity)
        {
            BattleEntity = battleEntity;
            battleEntity.Context.GetEventBus().AddListener<SelectSkillEvent>(OnSelectSkillEvent);
            battleEntity.Context.GetEventBus().AddListener<SkillCastEvent>(OnSkillCastEvent);
        }

        /// <summary>
        /// 选择技能事件回调
        /// </summary>
        /// <param name="selectSkillEvent"></param>
        private void OnSelectSkillEvent(SelectSkillEvent selectSkillEvent)
        {
            if (selectSkillEvent.BattleEntity != this.BattleEntity)
            {
                return;
            }

            // 根据技能信息获取动画类型
            SkillInfo skillInfo = BinaryDataManager.Instance.GetConfig<SkillInfoContainer>(E_ConfigLoadType.Editor).dataDic[selectSkillEvent.SkillId];

            switch ((E_SkillType)skillInfo.f_SkillType)
            {
                case E_SkillType.Monster:
                    SetAnimationState(E_AnimationType.PreNormalAttack);
                    break;
                case E_SkillType.NormalAttack:
                    SetAnimationState(E_AnimationType.PreNormalAttack);
                    break;
                case E_SkillType.CombatSkill:
                    SetAnimationState(E_AnimationType.PreBattleAttack);
                    break;
                case E_SkillType.UltimateSkill:
                    SetAnimationState(E_AnimationType.PreUltimateAttack);
                    break;
                case E_SkillType.EnhancedNormalAttack:
                    break;
                case E_SkillType.EnhancedCombatSkill:
                    break;
            }

            //LogManager.Log($"改变动画:{(E_SkillType)skillInfo.f_SkillType}");

            // NOTE：技能释放结束后，自动切换为待机状态;
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
