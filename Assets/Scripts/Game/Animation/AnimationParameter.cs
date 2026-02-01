using UnityEngine;

namespace Game.Animation
{
    /// <summary>
    /// ��������
    /// </summary>
    public class AnimationParameter
    {
        /// <summary>
        /// �ƶ�����hash��bool��
        /// </summary>
        public int IsRunHash { get; } = Animator.StringToHash("IsRun");

        /// <summary>
        /// �չ�Ԥ���ͷ�hash
        /// </summary>
        public int PreNormalAttackTriggerHash { get; } = Animator.StringToHash("PreNormalAttackTrigger");

        /// <summary>
        /// ��ͨ��������hash
        /// </summary>
        public int NormalAtkTirggerHash { get; } = Animator.StringToHash("NormalAtkTirgger");

        /// <summary>
        /// ս��Ԥ���ͷ�hash
        /// </summary>
        public int PreBattleAttackTriggerHash { get; } = Animator.StringToHash("PreBattleAttackTrigger");

        /// <summary>
        /// ս����������hash
        /// </summary>
        public int BattleAtkTriggerHash { get; } = Animator.StringToHash("BattleAtkTrigger");

        /// <summary>
        /// �սἼԤ���ͷ�hash
        /// </summary>
        public int PreUltimateAttackTriggerHash { get; } = Animator.StringToHash("PreUltimateAttackTrigger");

        /// <summary>
        /// �սἼ��������hash
        /// </summary>
        public int UltimateAtkTriggerHash { get; } = Animator.StringToHash("UltimateAtkTrigger");

        /// <summary>
        /// �ܻ�����hash
        /// </summary>
        public int HitTriggerHash { get; } = Animator.StringToHash("HitTrigger");

        /// <summary>
        /// ��������hash
        /// </summary>
        public int DeathTriggerHash { get; } = Animator.StringToHash("DeathTrigger");

        /// <summary>
        /// ��������hash
        /// </summary>
        public int RebirthTriggerHash { get; } = Animator.StringToHash("RebirthTrigger");

        /// <summary>
        /// ��������hash
        /// </summary>
        public int AttackTirggerHash { get; } = Animator.StringToHash("AttackTirgger");
    }
}
