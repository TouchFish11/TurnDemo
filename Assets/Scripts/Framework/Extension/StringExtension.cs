
/// <summary>
/// String拓展
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// 仅首字母大写，其余字符保留原格式
    /// </summary>
    public static string FirstLetterToUpper(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;
        if (input.Length == 1)
            return char.ToUpper(input[0]).ToString();
        return char.ToUpper(input[0]) + input.Substring(1);
    }
}
