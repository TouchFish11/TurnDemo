namespace HotUpdate.Game.Battle.UI
{
    /// <summary>
    /// 行动提示类型
    /// 用于控制战斗中"当前行动方"提示的显示状态
    /// </summary>
    public enum E_ActTipType : byte
    {
        /// <summary>
        /// 隐藏提示
        /// </summary>
        Hide,
        /// <summary>
        /// 玩家行动提示
        /// </summary>
        Player,
        /// <summary>
        /// 怪物行动提示
        /// </summary>
        Monster,
    }
}