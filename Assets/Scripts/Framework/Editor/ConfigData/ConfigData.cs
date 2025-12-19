using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 字段类型
/// </summary>
public enum E_FieldType
{
    None,
    Int,
    Float,
    String,
    Bool,
}

[Serializable]
public class ConfigData
{
    public string configName; // 类名
    public List<FieldTemplate> columns; // 列模板列表
    public List<EntryData> rows; // 行数据

    // 构造函数
    public ConfigData(string name)
    {
        configName = name;
        columns = new List<FieldTemplate>();
        rows = new List<EntryData>();
    }
}
