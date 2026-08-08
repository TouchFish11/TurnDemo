namespace Core.Editor.Generation
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 将访问修饰符枚举转换为对应的小写字符串
        /// 示例：E_AccessModifier.Public → "public"
        /// </summary>
        /// <param name="e_AccessModifier">要转换的访问修饰符枚举值</param>
        /// <returns>小写的修饰符字符串</returns>
        internal static string ToEnumString(this E_AccessModifier e_AccessModifier)
        {
            return e_AccessModifier.ToString().ToLower();
        }
    }
}
