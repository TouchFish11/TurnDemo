using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 行数据
/// </summary>
[Serializable]
public class EntryData
{
    // 字段名到值的映射
    private Dictionary<string, string> fieldToValueMap = new Dictionary<string, string>();

    public EntryData(List<FieldTemplate> fields)
    {
        // 初始化行数据为字段的默认值
        foreach (var col in fields)
        {
            switch (col.fieldType)
            {
                case E_FieldType.None:
                case E_FieldType.String:
                    fieldToValueMap.Add(col.fieldName, string.Empty);
                    break;
                case E_FieldType.Int:
                    fieldToValueMap.Add(col.fieldName, default(int).ToString());
                    break;
                case E_FieldType.Float:
                    fieldToValueMap.Add(col.fieldName, default(float).ToString());
                    break;
                case E_FieldType.Bool:
                    fieldToValueMap.Add(col.fieldName, default(bool).ToString());
                    break;
            }
        }
    }

    /// <summary>
    /// 通过字段名获取字段值
    /// </summary>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public string this[string fieldName] => fieldToValueMap[fieldName];

    /// <summary>
    /// 尝试添加
    /// </summary>
    /// <param name="fieldName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryAdd(string fieldName, string value)
    {
        return fieldToValueMap.TryAdd(fieldName, value);
    }

    /// <summary>
    /// 移除指定字段名/值对
    /// </summary>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public bool Remove(string fieldName)
    {
        return fieldToValueMap.Remove(fieldName);
    }

    /// <summary>
    /// 获取所有的字段名
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> GetFieldNames()
    {
        return fieldToValueMap.Keys;
    }

    /// <summary>
    /// 获取字段值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public string GetValue(string fieldName)
    {
        if (fieldToValueMap == null)
        {
            return string.Empty;
        }

        if (fieldToValueMap.TryGetValue(fieldName, out string value))
        {
            return value;
        }

        Debug.Log($"获取字段值失败，不存在该字段：{fieldName}，已返回默认值");
        return string.Empty;
    }

    /// <summary>
    /// 设置字段值
    /// </summary>
    /// <param name="fieldName"></param>
    /// <param name="value"></param>
    public void SetValue(string fieldName, string value)
    {
        if (!fieldToValueMap.ContainsKey(fieldName))
        {
            Debug.Log($"设置字段值失败，该字段不存在：{fieldName}");
        }
        fieldToValueMap[fieldName] = value;
    }
}
