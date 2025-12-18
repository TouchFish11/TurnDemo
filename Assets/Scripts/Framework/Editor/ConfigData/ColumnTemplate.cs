using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 列模板
/// </summary>
[Serializable]
public class ColumnTemplate
{
    public E_FieldType fieldType; // 字段类型
    public string fieldName; // 字段名（如taskId）
    //public string defaultValue; // 默认值（字符串存储，方便序列化）
    public string fieldDescription;    // 字段描述
    public bool key;    // 是否为主键

    public ColumnTemplate(string fieldName, E_FieldType fieldType)
    {
        this.fieldName = fieldName;
        this.fieldType = fieldType;
        object defaultValue = GetTypeDefault(fieldType);
        //this.defaultValue = defaultValue == null ? string.Empty : defaultValue.ToString();
    }

    ///// <summary>
    ///// 将字符串默认值转换为实际对象
    ///// </summary>
    ///// <returns></returns>
    //public object GetDefaultValue()
    //{
    //    return ConvertValue(defaultValue, fieldType);
    //}

    ///// <summary>
    ///// 类型转换：字符串→实际类型
    ///// </summary>
    ///// <param name="value"></param>
    ///// <param name="type"></param>
    ///// <returns></returns>
    //public static object ConvertValue(E_FieldType type)
    //{
    //    try
    //    {
    //        return type switch
    //        {
    //            E_FieldType.Int => int.Parse(value),
    //            E_FieldType.Float => float.Parse(value),
    //            E_FieldType.Bool => bool.Parse(value),
    //            E_FieldType.String => value,
    //            _ => null,
    //        };
    //    }
    //    catch
    //    {
    //        // 转换失败返回类型默认值
    //        Debug.Log($"转换失败：{type}，值：{value}");
    //        return GetTypeDefault(type);
    //    }
    //}

    /// <summary>
    /// 获取类型的默认值
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object GetTypeDefault(E_FieldType type)
    {
        return type switch
        {
            E_FieldType.Int => default(int),
            E_FieldType.Float => default(float),
            E_FieldType.Bool => default(bool),
            E_FieldType.String => string.Empty,
            _ => null,
        };
    }
}
