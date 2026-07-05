using HotUpdate.Base.Animation;
using HotUpdate.Base.Component;
using HotUpdate.Base.Enums;
using HotUpdate.Game.Battle.Core;
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
        
        public Animator Animator => _battleAnimationComponentCore.GetAnimator();
        
        public AnimationParameter Parameter => _battleAnimationComponentCore.GetParameter();

        protected override void OnBattleInit()
        {
            _battleAnimationComponentCore = (BattleAnimationComponentCore)ComponentCore;
            // // 初始化默认动画类型：玩家默认预普通攻击动画，其他实体（怪物）默认无动画
            // _battleAnimationComponentCore.AnimationType = BattleEntity is IPlayerObject ? E_AnimationType.PreNormalAttack : E_AnimationType.None;
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
            //_battleAnimationComponentCore.AnimationType = BattleEntity is IPlayerObject ? E_AnimationType.PreNormalAttack : E_AnimationType.None;
        }
        
        protected override void OnBattleDestroy()
        {
            _battleAnimationComponentCore = null;
        }
    }
}