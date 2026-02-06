using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CustomEditor.ScriptGeneration;
using Editor.ConfigData;
using UnityEditor;
using UnityEngine;

namespace Editor.Generation.Detail
{
    /// <summary>
    /// 配置数据生成器
    /// 根据配置数据结构自动生成数据类、容器类和二进制数据文件
    /// </summary>
    public class ConfigDataGenerator : ClassGenerator
    {
        /// <summary>
        /// 数据结构类在编辑器中的存储文件夹路径
        /// </summary>
        private static readonly string DataclassEditorSavePath = $"{Application.dataPath}/Scripts/Config/EditorInfo/Info/";

        /// <summary>
        /// 数据容器类在编辑器中的存储文件夹路径
        /// </summary>
        private static readonly string DataContainerEditorSavePath = $"{Application.dataPath}/Scripts/Config/EditorInfo/Container/";

        /// <summary>
        /// 数据表文件在编辑器中的存储文件夹路径
        /// </summary>
        private static readonly string TableInfoEditorSavePath = $"{Application.dataPath}/Editor/ArtRes/GameConfig/Editor/";

        // 配置数据对象
        private readonly ConfigData.ConfigData configData;

        protected override string NameSpace => string.Empty;

        protected override string Note { get; set; }

        public ConfigDataGenerator(ConfigData.ConfigData configData)
        {
            this.configData = configData;
        }

        public override void GenerateScript()
        {
            // 生成数据结构类
            GenerateDataClass();
            // 生成数据容器类
            GenerateDataContainer();
            // 生成二进制数据文件
            GenerateDataBinary();
        }

        /// <summary>
        /// 生成数据结构类
        /// </summary>
        private void GenerateDataClass()
        {
            // 检查存储目录是否存在，不存在则创建
            if (!Directory.Exists(DataclassEditorSavePath))
            {
                Directory.CreateDirectory(DataclassEditorSavePath);
            }

            // 获取所有字段名称
            List<string> fieldNames = GetTotalFieldName();
            if (fieldNames.Count == 0)
            {
                Debug.Log($"生成数据结构类失败：未检测到任何字段名");
                return;
            }

            // 获取所有字段类型
            List<string> fieldTypes = GetTotalFieldType();
            if (fieldTypes.Count == 0)
            {
                Debug.Log($"生成数据结构类失败：未检测到任何字段类型");
                return;
            }

            // 拼接数据结构类的代码字符串
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"public class {configData.configName}");
            sb.AppendLine("{");
            for (int i = 0; i < fieldTypes.Count; i++)
            {
                sb.AppendLine($"\tpublic {fieldTypes[i]} {fieldNames[i]};");
            }
            sb.AppendLine("}");

            // 再次检查目录（双重校验）
            if (!Directory.Exists(DataclassEditorSavePath))
            {
                Directory.CreateDirectory(DataclassEditorSavePath);
            }

            // 将生成的代码写入CS文件
            File.WriteAllText($"{DataclassEditorSavePath}{configData.configName}.cs", sb.ToString());
            // 刷新Unity资源数据库
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 生成数据容器类（包含字典用于索引数据）
        /// </summary>
        private void GenerateDataContainer()
        {
            // 获取主键字段的类型和索引
            (E_FieldType type, int keyIndex) = GetKeyFieldType();
            if (type == E_FieldType.None)
            {
                Debug.Log($"生成数据容器类失败：字段{configData.columns[keyIndex].fieldName}类型为None");
                return;
            }

            // 将枚举类型转为小写字符串作为字典键类型
            string keyType = type.ToString().ToLower();

            // 拼接数据容器类的代码字符串
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"using System.Collections.Generic;");
            sb.AppendLine($"public class {configData.configName}Container");
            sb.AppendLine("{");
            sb.AppendLine($"\tpublic Dictionary<{keyType}, {configData.configName}> dataDic = new Dictionary<{keyType}, {configData.configName}>();");
            sb.AppendLine("}");

            // 检查存储目录是否存在，不存在则创建
            if (!Directory.Exists(DataContainerEditorSavePath))
            {
                Directory.CreateDirectory(DataContainerEditorSavePath);
            }

            // 将生成的代码写入CS文件
            File.WriteAllText($"{DataContainerEditorSavePath}{configData.configName}Container.cs", sb.ToString());
            // 刷新Unity资源数据库
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 生成二进制数据文件（用于运行时加载配置）
        /// </summary>
        private void GenerateDataBinary()
        {
            // 检查存储目录是否存在，不存在则创建
            if (!Directory.Exists(TableInfoEditorSavePath))
            {
                Directory.CreateDirectory(TableInfoEditorSavePath);
            }

            // 检查配置行数据是否为空
            if (configData.rows.Count == 0)
            {
                Debug.Log($"生成二进制数据文件失败：未检测到任何行数据");
                return;
            }

            // 创建文件流写入二进制数据
            using FileStream fs = new FileStream($"{TableInfoEditorSavePath}{configData.configName}.bytes", FileMode.OpenOrCreate, FileAccess.Write);
            // 第一步：写入行数据总数（int类型，4字节）
            fs.Write(BitConverter.GetBytes(configData.rows.Count), 0, 4);
            // 第二步：写入主键字段名称
            string keyName = GetKeyFIeldName();
            byte[] keyBytes = Encoding.UTF8.GetBytes(keyName);
            // 先写入字符串长度（int类型，4字节）
            fs.Write(BitConverter.GetBytes(keyBytes.Length), 0, 4);
            // 再写入字符串字节数据
            fs.Write(keyBytes, 0, keyBytes.Length);
            
            // 第三步：逐行写入所有字段的二进制数据
            for (int i = 0; i < configData.rows.Count; i++)
            {
                EntryData rowData = configData.rows[i];

                // 遍历当前行的所有列
                for (int j = 0; j < configData.columns.Count; j++)
                {
                    E_FieldType fieldType = configData.columns[j].fieldType;
                    string fieldName = configData.columns[j].fieldName;

                    // 根据字段类型写入对应二进制数据
                    switch (fieldType)
                    {
                        case E_FieldType.None:
                            Debug.LogError($"字段类型不能为None，字段名称：{configData.columns[j].fieldName}");
                            return;
                        case E_FieldType.Int:
                            fs.Write(BitConverter.GetBytes(int.Parse(rowData.GetValue(fieldName))), 0, 4);
                            break;
                        case E_FieldType.Float:
                            fs.Write(BitConverter.GetBytes(float.Parse(rowData.GetValue(fieldName))), 0, 4);
                            break;
                        case E_FieldType.Bool:
                            fs.Write(BitConverter.GetBytes(bool.Parse(rowData.GetValue(fieldName))), 0, 1);
                            break;
                        case E_FieldType.String:
                            byte[] bytes = Encoding.UTF8.GetBytes(rowData.GetValue(fieldName) ?? "");
                            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4); // 先写字符串长度
                            fs.Write(bytes, 0, bytes.Length); // 再写字节数据
                            break;
                    }
                }
            }
            // 关闭文件流
            fs.Close();
            // 刷新Unity资源数据库
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 获取主键字段的类型和索引
        /// </summary>
        /// <returns>主键字段类型、主键字段索引</returns>
        private (E_FieldType fieldType, int keyIndex) GetKeyFieldType()
        {
            // 遍历所有列，找到标记为主键的列
            for (int i = 0; i < configData.columns.Count; i++)
            {
                if (configData.columns[i].key)
                {
                    return (configData.columns[i].fieldType, i);
                }
            }

            // 未指定主键时，默认使用第一列作为主键并给出提示
            Debug.Log($"未指定主键，已默认将第一列{configData.columns[0].fieldName}设为主键");
            return (configData.columns[0].fieldType, 0);
        }

        /// <summary>
        /// 获取主键字段的名称
        /// </summary>
        /// <returns>主键字段名称</returns>
        private string GetKeyFIeldName()
        {
            // 遍历所有列，找到标记为主键的列
            for (int i = 0; i < configData.columns.Count; i++)
            {
                if (configData.columns[i].key)
                {
                    return configData.columns[i].fieldName;
                }
            }

            // 未指定主键时，默认使用第一列作为主键并给出提示
            Debug.Log($"未指定主键，已默认将第一列{configData.columns[0].fieldName}设为主键");
            return configData.columns[0].fieldName;
        }

        /// <summary>
        /// 获取所有字段名称
        /// </summary>
        /// <returns>字段名称列表</returns>
        private List<string> GetTotalFieldName()
        {
            // 行数据为空时返回空列表
            if (configData.rows.Count == 0)
            {
                return new List<string>();
            }

            // 从第一行数据中获取所有字段名称
            return new List<string>(configData.rows[0].GetFieldNames());
        }

        /// <summary>
        /// 获取所有字段类型（转为小写字符串）
        /// </summary>
        /// <returns>字段类型列表</returns>
        private List<string> GetTotalFieldType()
        {
            // 列配置为空时返回空列表
            if (configData.columns.Count == 0)
            {
                return new List<string>();
            }

            List<string> fieldTypes = new List<string>();
            foreach (var item in configData.columns)
            {
                // 将枚举类型转为小写字符串（如Int -> int）
                fieldTypes.Add(item.fieldType.ToString().ToLower());
            }

            return fieldTypes;
        }
    }
}