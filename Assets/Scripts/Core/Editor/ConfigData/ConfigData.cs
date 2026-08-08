using System;
using System.Collections.Generic;

namespace Core.Editor.ConfigData
{
    /// <summary>
    /// 配置字段的类型枚举
    /// 用于标识配置表中列的数值类型
    /// </summary>
    public enum E_FieldType
    {
        /// <summary>
        /// 未定义类型（默认/无效值）
        /// </summary>
        None,
        /// <summary>
        /// 整数类型（对应C# int）
        /// </summary>
        Int,
        /// <summary>
        /// 浮点类型（对应C# float）
        /// </summary>
        Float,
        /// <summary>
        /// 字符串类型（对应C# string）
        /// </summary>
        String,
        /// <summary>
        /// 布尔类型（对应C# bool）
        /// </summary>
        Bool,
    }

    /// <summary>
    /// 配置数据表的核心数据模型，用于存储单张配置表的完整信息
    /// 包含配置表名称、列定义、行数据三部分
    /// </summary>
    [Serializable] // 标记为可序列化，支持Unity序列化/反序列化或JSON等序列化场景
    public class ConfigData
    {
        /// <summary>
        /// 配置表对应的类名（也可理解为配置表名称）
        /// 用于代码生成、配置表标识等场景
        /// </summary>
        public string configName;

        /// <summary>
        /// 配置表的列模板列表，每一项描述一列的元信息（如字段名、字段类型等）
        /// </summary>
        public List<FieldTemplate> columns;

        /// <summary>
        /// 配置表的行数据列表，每一项对应一行具体的业务数据
        /// </summary>
        public List<EntryData> rows;

        /// <summary>
        /// 配置数据模型的构造函数
        /// </summary>
        /// <param name="name">初始化的配置表名称（类名）</param>
        public ConfigData(string name)
        {
            // 初始化配置表名称
            configName = name;
            // 初始化列模板列表，避免空引用
            columns = new List<FieldTemplate>();
            // 初始化行数据列表，避免空引用
            rows = new List<EntryData>();
        }
    }
}