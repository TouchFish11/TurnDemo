using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.ConfigData
{
    /// <summary>
    /// 配置项数据类
    /// 用于存储单条配置数据的字段名与字段值的映射关系，支持多种基础数据类型的默认值初始化、值的获取与设置
    /// </summary>
    [Serializable]
    public class EntryData
    {
        // 字段名到字段值的映射字典，统一以字符串形式存储各类数据值
        private Dictionary<string, string> fieldToValueMap = new Dictionary<string, string>();

        /// <summary>
        /// 构造函数：根据字段模板列表初始化配置项数据
        /// 为每个字段初始化对应类型的默认值并存入映射字典
        /// </summary>
        /// <param name="fields">字段模板列表，包含字段名、字段类型等元信息</param>
        public EntryData(List<FieldTemplate> fields)
        {
            // 遍历所有字段模板，按字段类型初始化默认值
            foreach (var col in fields)
            {
                switch (col.fieldType)
                {
                    case E_FieldType.None:    // 无类型
                    case E_FieldType.String:  // 字符串类型
                        fieldToValueMap.Add(col.fieldName, string.Empty); // 默认值：空字符串
                        break;
                    case E_FieldType.Int:     // 整数类型
                        fieldToValueMap.Add(col.fieldName, 0.ToString()); // 默认值：0
                        break;
                    case E_FieldType.Float:   // 浮点类型
                        fieldToValueMap.Add(col.fieldName, 0.0f.ToString()); // 默认值：0.0f
                        break;
                    case E_FieldType.Bool:    // 布尔类型
                        fieldToValueMap.Add(col.fieldName, false.ToString()); // 默认值：false
                        break;
                }
            }
        }

        /// <summary>
        /// 索引器：通过字段名快速获取对应的字段值
        /// 注：若字段名不存在，会抛出KeyNotFoundException异常
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <returns>字段对应的字符串值</returns>
        public string this[string fieldName] => fieldToValueMap[fieldName];

        /// <summary>
        /// 尝试添加新的字段名-值映射关系
        /// 仅当字段名不存在时添加成功，避免覆盖已有数据
        /// </summary>
        /// <param name="fieldName">要添加的字段名称</param>
        /// <param name="value">字段对应的字符串值</param>
        /// <returns>添加成功返回true，字段已存在返回false</returns>
        public bool TryAdd(string fieldName, string value)
        {
            return fieldToValueMap.TryAdd(fieldName, value);
        }

        /// <summary>
        /// 移除指定字段名对应的映射关系
        /// </summary>
        /// <param name="fieldName">要移除的字段名称</param>
        /// <returns>移除成功返回true，字段不存在返回false</returns>
        public bool Remove(string fieldName)
        {
            return fieldToValueMap.Remove(fieldName);
        }

        /// <summary>
        /// 获取所有已存储的字段名称集合
        /// </summary>
        /// <returns>字段名的可枚举集合</returns>
        public IEnumerable<string> GetFieldNames()
        {
            return fieldToValueMap.Keys;
        }

        /// <summary>
        /// 安全获取指定字段的值
        /// 若字典为空、字段不存在时返回空字符串，并打印日志提示
        /// </summary>
        /// <param name="fieldName">要获取值的字段名称</param>
        /// <returns>字段对应的字符串值；获取失败时返回空字符串</returns>
        public string GetValue(string fieldName)
        {
            // 字典未初始化时返回空字符串
            if (fieldToValueMap == null)
            {
                return string.Empty;
            }

            // 字段存在时返回对应值
            if (fieldToValueMap.TryGetValue(fieldName, out string value))
            {
                return value;
            }

            // 字段不存在时打印警告日志并返回空字符串
            Debug.Log($"获取字段值失败，不存在该字段：{fieldName}，已返回默认值");
            return string.Empty;
        }

        /// <summary>
        /// 设置指定字段的值
        /// 若字段不存在，打印警告日志但仍尝试赋值（避免字典键不存在异常）
        /// </summary>
        /// <param name="fieldName">要设置值的字段名称</param>
        /// <param name="value">要设置的字符串值</param>
        public void SetValue(string fieldName, string value)
        {
            // 字段不存在时打印警告日志
            if (!fieldToValueMap.ContainsKey(fieldName))
            {
                Debug.Log($"设置字段值失败，该字段不存在：{fieldName}");
            }
            // 无论字段是否存在，都执行赋值（不存在时会自动添加键值对）
            fieldToValueMap[fieldName] = value;
        }
    }
}