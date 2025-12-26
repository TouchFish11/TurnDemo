
using Framework;
using Game.Battle;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 动画组件
    /// </summary>
    public class AnimComponent : BattleComponent
    {
        // 动画控制器
        private Animator animator;
        // 动画参数
        private AnimationParameter animationArg;
        // 动画类型
        private AnimationType currentAnimationType = AnimationType.None;

        protected void Awake()
        {
            animationArg = new AnimationParameter();

            //this.BattleEntity.GetComponent<InputComponent>().OnMouseLeftClick += OnAttack;
        }

        public override void BattleInit(IBattleEntityObject battleEntity)
        {
            base.BattleInit(battleEntity);
            animator = this.BattleEntity.GetComponentInChildren<Animator>();
            this.BattleEntity.GetComponent<InputComponent>().OnKeyInputChanged += OnMove;
            //ServiceLocator.Instance.Get<IBattleManager>().GetContext().GetEventBus().AddListener<SelectSkillEvent>(OnSelectSkillEvent);
        }

        private void OnSelectSkillEvent(SelectSkillEvent selectSkillEvent)
        {
            // 根据技能信息获取动画类型
            SkillInfo skillInfo = BinaryDataMgr.Instance.GetConfig<SkillInfoContainer>(E_ConfigLoadType.Editor).dataDic[selectSkillEvent.SkillId];
            SetAnimationState(AnimationType.None/* 技能配置动画 */);
        }

        /// <summary>
        /// 设置动画类型
        /// </summary>
        /// <param name="animationType"></param>
        public void SetAnimationState(AnimationType animationType)
        {
            if (animationType == currentAnimationType)
            {
                return;
            }

            switch (animationType)
            {
                case AnimationType.None:
                    break;
                case AnimationType.Idle:
                    animator.SetBool(animationArg.IsRunHash, false);
                    break;
                case AnimationType.Run:
                    animator.SetBool(animationArg.IsRunHash, true);
                    break;
                case AnimationType.NormalAttack:
                    animator.SetTrigger(animationArg.NormalAtkTirggerHash);
                    break;
                case AnimationType.PreBattleAtk:
                    animator.SetBool(animationArg.IsPreBattleAtkHash, true);
                    break;
                case AnimationType.BattleAttack:
                    animator.SetTrigger(animationArg.BattleAtkTriggerHash);
                    break;
                case AnimationType.UltimateAttack:
                    animator.SetTrigger(animationArg.UltimateAtkTriggerHash);
                    break;
                case AnimationType.Hit:
                    animator.SetTrigger(animationArg.HitTriggerHash);
                    break;
                case AnimationType.Death:
                    animator.SetTrigger(animationArg.DeathTriggerHash);
                    break;
                case AnimationType.Rebirth:
                    animator.SetTrigger(animationArg.RebirthTriggerHash);
                    break;
                default:
                    break;
            }
            currentAnimationType = animationType;
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="inputDir"></param>
        public void OnMove(Vector3 inputDir)
        {
            SetAnimationState(inputDir != Vector3.zero ? AnimationType.Run : AnimationType.Idle);
        }

        /// <summary>
        /// 攻击
        /// </summary>
        public void OnAttack()
        {
            SetAnimationState(AnimationType.NormalAttack);
        }
    }
}
