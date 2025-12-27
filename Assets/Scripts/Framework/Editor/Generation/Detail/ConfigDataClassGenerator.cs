using CustomEditor.ScriptGeneration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 配置数据类生成器
/// ——生成数据结构、容器、配置类脚本和二进制数据
/// </summary>
public class ConfigDataClassGenerator : ClassGenerator
{
    /// <summary>
    /// 数据结构类脚本在编辑器的存储的文件夹
    /// </summary>
    private static readonly string DataclassEditorSavePath = $"{Application.dataPath}/Scripts/ConfigInfo/Info/";

    /// <summary>
    /// 数据容器类脚本在编辑器的存储的文件夹
    /// </summary>
    private static readonly string DataContainerEditorSavePath = $"{Application.dataPath}/Scripts/ConfigInfo/Container/";

    /// <summary>
    /// 表数据文件在编辑器的存储文件夹
    /// </summary>
    private static readonly string TableInfoEditorSavePath = $"{Application.dataPath}/Editor/ArtRes/GameData/";

    private readonly ConfigData configData;

    public override string FilePath => string.Empty;

    protected override string Note => string.Empty;

    protected override string NameSpace => string.Empty;

    public ConfigDataClassGenerator(ConfigData configData)
    {
        this.configData = configData;
    }

    public override void GenerateScript()
    {
        GenerateDataClass();

        GenerateDataContainer();

        GenerateDataBinary();
    }

    /// <summary>
    /// 生成数据结构类
    /// </summary>
    private void GenerateDataClass()
    {
        if (!Directory.Exists(DataclassEditorSavePath))
        {
            Directory.CreateDirectory(DataclassEditorSavePath);
        }

        List<string> fieldNames = GetTotalFieldName();
        if (fieldNames.Count == 0)
        {
            Debug.Log($"生成数据结构类失败，不存在任何字段名");
            return;
        }

        List<string> fieldTypes = GetTotalFieldType();
        if (fieldTypes.Count == 0)
        {
            Debug.Log($"生成数据结构类失败，不存在任何变量类型");
            return;
        }

        // 生成数据结构类脚本，就是通过代码进行字符串拼接，然后存进文件
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"public class {configData.configName}");
        sb.AppendLine("{");
        for (int i = 0; i < fieldTypes.Count; i++)
        {
            sb.AppendLine($"\tpublic {fieldTypes[i]} {fieldNames[i]};");
        }
        sb.AppendLine("}");

        // 判断数据结构类存储文件路径是否存在
        if (!Directory.Exists(DataclassEditorSavePath))
        {
            Directory.CreateDirectory(DataclassEditorSavePath);
        }

        //保存文件
        File.WriteAllText($"{DataclassEditorSavePath}{configData.configName}.cs", sb.ToString());
        //刷新窗口
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 生成数据容器类
    /// </summary>
    private void GenerateDataContainer()
    {
        // 获取主键类型
        (E_FieldType type, int keyIndex) = GetKeyFieldType();
        if (type == E_FieldType.None)
        {
            Debug.Log($"生成数据类容器失败，字段：{configData.columns[keyIndex].fieldName}，类型为None");
            return;
        }

        string keyType = type.ToString().ToLower();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"using System.Collections.Generic;");
        sb.AppendLine($"public class {configData.configName}Container");
        sb.AppendLine("{");
        sb.AppendLine($"\tpublic Dictionary<{keyType}, {configData.configName}> dataDic = new Dictionary<{keyType}, {configData.configName}>();");
        sb.AppendLine("}");

        //判断数据结构容器类存储文件路径是否存在
        if (!Directory.Exists(DataContainerEditorSavePath))
        {
            Directory.CreateDirectory(DataContainerEditorSavePath);
        }

        //保存到文件中
        File.WriteAllText($"{DataContainerEditorSavePath}{configData.configName}Container.cs", sb.ToString());
        //刷新窗口
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 生成数据二进制
    /// </summary>
    private void GenerateDataBinary()
    {
        // 路径不存在，创建路径
        if (!Directory.Exists(TableInfoEditorSavePath))
        {
            Directory.CreateDirectory(TableInfoEditorSavePath);
        }

        if (configData.rows.Count == 0)
        {
            Debug.Log($"生成数据二进制失败，不存在任何配置数据");
            return;
        }

        using FileStream fs = new FileStream($"{TableInfoEditorSavePath}{configData.configName}.bytes", FileMode.OpenOrCreate, FileAccess.Write);
        // 先存储需要写多少行数据，方便读取
        fs.Write(BitConverter.GetBytes(configData.rows.Count), 0, 4);
        // 存储主键变量名
        string keyName = GetKeyFIeldName();
        byte[] keyBytes = Encoding.UTF8.GetBytes(keyName);
        //存储字符串长度
        fs.Write(BitConverter.GetBytes(keyBytes.Length), 0, 4);
        //存储字符串
        fs.Write(keyBytes, 0, keyBytes.Length);
        // 遍历所有条目
        for (int i = 0; i < configData.rows.Count; i++)
        {
            EntryData rowData = configData.rows[i];

            // 遍历所有类型
            for (int j = 0; j < configData.columns.Count; j++)
            {
                E_FieldType fieldType = configData.columns[j].fieldType;
                string fieldName = configData.columns[j].fieldName;

                switch (fieldType)
                {
                    case E_FieldType.None:
                        Debug.LogError($"字段类型不能为None，字段名：{configData.columns[j].fieldName}");
                        return;
                    case E_FieldType.Int:
                        fs.Write(BitConverter.GetBytes(int.Parse(rowData.GetValue(fieldName).ToString())), 0, 4);
                        break;
                    case E_FieldType.Float:
                        fs.Write(BitConverter.GetBytes(float.Parse(rowData.GetValue(fieldName).ToString())), 0, 4);
                        break;
                    case E_FieldType.Bool:
                        fs.Write(BitConverter.GetBytes(bool.Parse(rowData.GetValue(fieldName).ToString())), 0, 1);
                        break;
                    case E_FieldType.String:
                        byte[] bytes = Encoding.UTF8.GetBytes(rowData.GetValue(fieldName) ?? "");
                        fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                        fs.Write(bytes, 0, bytes.Length);
                        break;
                }
            }
        }
        fs.Close();
        fs.Dispose();

        // 刷新窗口
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 获取主键的字段类型
    /// </summary>
    /// <returns></returns>
    private (E_FieldType fieldType, int keyIndex) GetKeyFieldType()
    {
        for (int i = 0; i < configData.columns.Count; i++)
        {
            if (configData.columns[i].key)
            {
                return (configData.columns[i].fieldType, i);
            }
        }

        // 未指定则以第一个字作为容器主键
        Debug.Log($"未指定主键，已将第一个字段{configData.columns[0].fieldName}的变量类型作为主键");
        return (configData.columns[0].fieldType, 0);
    }

    /// <summary>
    /// 获取主键的字段名
    /// </summary>
    /// <returns></returns>
    private string GetKeyFIeldName()
    {
        for (int i = 0; i < configData.columns.Count; i++)
        {
            if (configData.columns[i].key)
            {
                return configData.columns[i].fieldName;
            }
        }

        // 未指定则以第一个字段作为容器主键
        Debug.Log($"未指定主键，已将第一个字段{configData.columns[0].fieldName}的字段名作为主键");
        return configData.columns[0].fieldName;
    }

    /// <summary>
    /// 获取所有的字段名
    /// </summary>
    /// <returns></returns>
    private List<string> GetTotalFieldName()
    {
        // 获取所有字段名
        if (configData.rows.Count == 0)
        {
            return new List<string>();
        }

        return new List<string>(configData.rows[0].GetFieldNames());
    }

    /// <summary>
    /// 获取所有的字段类型
    /// </summary>
    /// <returns></returns>
    private List<string> GetTotalFieldType()
    {
        // 获取所有变量类型
        if (configData.columns.Count == 0)
        {
            return new List<string>();
        }

        List<string> fieldTypes = new List<string>();
        foreach (var item in configData.columns)
        {
            fieldTypes.Add(item.fieldType.ToString().ToLower());
        }

        return fieldTypes;
    }
}
