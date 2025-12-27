namespace Framework
{
    /// <summary>
    /// 行为枚举
    /// </summary>
    public enum E_Action : byte
    {
        /// <summary>
        /// 无
        /// </summary>
        None,

        //自定义行为
        Up,
        Down,
        Left,
        Right,
        NormalAttack,
        Initeract,
        MouseMove,
        ScrollZoom,
        MouseVisible,
    }
}
