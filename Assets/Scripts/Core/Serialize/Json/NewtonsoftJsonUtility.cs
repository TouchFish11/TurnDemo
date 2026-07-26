using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Serialize.Json
{
    /// <summary>
    /// Json工具类
    /// </summary>
    public static class NewtonsoftJsonUtility
    {
        /// <summary>
        /// 处理类型名称、格式化设置、枚举字符串
        /// </summary>
        public static readonly JsonSerializerSettings DefaultSerializerSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter()
            }
        };

        public static readonly JsonSerializerSettings CatalogSerializerSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter()
            },
            SerializationBinder = new CatalogSerializationBinder()
        };
    }
}
