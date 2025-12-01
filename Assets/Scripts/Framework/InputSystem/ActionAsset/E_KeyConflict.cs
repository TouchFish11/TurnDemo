namespace Framework
{
    /// <summary>
    /// 键位冲突
    /// </summary>
    public enum E_KeyConflict : byte
    {
        /// <summary>
        /// 特殊键位冲突
        /// </summary>
        SpecialKey,
        /// <summary>
        /// 相同按键冲突
        /// </summary>
        ExistKey,
        /// <summary>
        /// 非键盘按键冲突
        /// </summary>
        NotKeyboard,
        /// <summary>
        /// 改键结束
        /// </summary>
        Over,
    }
}
