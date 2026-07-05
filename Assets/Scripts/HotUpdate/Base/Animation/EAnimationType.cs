namespace HotUpdate.Base.Animation
{
    /// <summary>
    /// 动画类型
    /// </summary>
    public enum EAnimationType : byte
    {
        None,
        Idle = 1,
        Run = 2,
        PreNormalAttack = 3,
        NormalAttack = 4,
        PreBattleAttack = 5,
        BattleAttack = 6,
        PreUltimateAttack = 7,
        UltimateAttack = 8,
        Hit,
        Death,
        Rebirth,
        Attack,
    }
}
