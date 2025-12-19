using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 字段模板
/// </summary>
[Serializable]
public class FieldTemplate
{
    public E_FieldType fieldType; // 字段类型
    public string fieldName; // 字段名（如taskId）
    public string fieldDescription;    // 字段描述
    public bool key;    // 是否为主键

    public FieldTemplate(string fieldName, E_FieldType fieldType)
    {
        this.fieldName = fieldName;
        this.fieldType = fieldType;
    }
}
