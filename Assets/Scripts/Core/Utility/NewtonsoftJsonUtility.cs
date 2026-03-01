using Newtonsoft.Json;

namespace Core.Utility
{
    /// <summary>
    /// Json工具类
    /// </summary>
    public static class NewtonsoftJsonUtility
    {
        /// <summary>
        /// 处理类型名称、格式化设置
        /// </summary>
        public static readonly JsonSerializerSettings SerializerSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented,
        };
    }
}
