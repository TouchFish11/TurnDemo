using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RowData
{
    // 字段名映射值
    public Dictionary<string, string> fieldToValueMap = new Dictionary<string, string>();

    public RowData(List<ColumnTemplate> columns)
    {
        // 初始化行数据为列的默认值
        foreach (var col in columns)
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

    public bool TryAdd(string fieldName, string value)
    {
        return fieldToValueMap.TryAdd(fieldName, value);
    }

    public bool TryGetValue(string fieldName, out string value)
    {
        if (fieldToValueMap.TryGetValue(fieldName, out value))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取字段值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public string GetValue(string fieldName)
    {
        return fieldToValueMap[fieldName];
    }

    /// <summary>
    /// 设置字段值
    /// </summary>
    /// <param name="fieldName"></param>
    /// <param name="value"></param>
    public void SetValue(string fieldName, string value)
    {
        if (fieldToValueMap.ContainsKey(fieldName))
        {
            fieldToValueMap[fieldName] = value;
        }
    }
}
