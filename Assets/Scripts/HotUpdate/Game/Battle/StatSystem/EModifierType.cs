namespace HotUpdate.Game.Battle.StatSystem
{
    public enum EModifierType : byte
    {
        /// <summary>
        /// 固定值
        /// </summary>
        Flat,
        
        /// <summary>
        /// 百分比
        /// </summary>
        Percent,
        
        /// <summary>
        /// 属性转换
        /// </summary>
        Conversion,
    }
}
