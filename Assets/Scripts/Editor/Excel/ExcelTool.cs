using System;
using System.Data;
using System.IO;
using System.Text;
using Excel;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor.Excel
{
    /// <summary>
    /// Excel解析工具
    /// </summary>
    public static class ExcelTool
    {
        /// <summary>
        /// Excel文件在编辑器的读取路径
        /// </summary>
        private static readonly string ExcelEditorLoadPath = $"{Application.dataPath}/Excel/";

        /// <summary>
        /// 数据结构类脚本在编辑器的存储的文件夹
        /// </summary>
        private static readonly string DataclassEditorSavePath = $"{Application.dataPath}/Scripts/ExcelInfo/Info/";

        /// <summary>
        /// 数据容器类脚本在编辑器的存储的文件夹
        /// </summary>
        private static readonly string DataContainerEditorSavePath = $"{Application.dataPath}/Scripts/ExcelInfo/Container/";

        /// <summary>
        /// 表数据文件在编辑器的存储文件夹
        /// </summary>
        private static readonly string TableInfoEditorSavePath = $"{Application.dataPath}/Editor/ArtRes/GameData/";

        /// <summary>
        /// Excek数据内容开始行号
        /// </summary>
        private const int BeginIndex = 4;

        [MenuItem("GameTool/Excel/GenerateExcelData")]
        private static void GenerateExcel()
        {
            DirectoryInfo directoryInfo = null;
            // 获取该路径下的某个文件夹
            if (!Directory.Exists(ExcelEditorLoadPath))
            {
                if (EditorUtility.DisplayDialog("注意", ExcelEditorLoadPath + "该路径文件夹不存在，是否自动创建?", "确定"))
                {
                    directoryInfo = Directory.CreateDirectory(ExcelEditorLoadPath);
                    AssetDatabase.Refresh();
                }
            }
            else
            {
                directoryInfo = Directory.CreateDirectory(ExcelEditorLoadPath);
            }

            // 获取该文件夹下的所有文件信息
            if (directoryInfo == null)
            {
                return;
            }
            
            var fileInfos = directoryInfo.GetFiles();

            // 遍历所有文件信息
            foreach (var fileInfo in fileInfos)
            {
                // 不是Excel文件就不处理
                if (fileInfo.Extension != ".xlsx")
                    continue;

                // Excel数据表容器类
                DataTableCollection collection;
                // 将文件信息以流的形式读取
                using (var fs = fileInfo.Open(FileMode.Open, FileAccess.Read))
                {
                    // 获取该Excel文件中的所有表
                    var reader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                    collection = reader.AsDataSet().Tables;
                    fs.Close();
                }

                // 遍历每一张表
                foreach (DataTable table in collection)
                {
                    // 生成数据结构类
                    GenerateExcelDataClass(table);
                    // 生成数据结构容器类
                    GenerateExcelContainer(table);
                    // 生成二进制数据
                    GenerateExcelBinary(table);
                }
            }
        }

        /// <summary>
        /// 生成数据结构类脚本
        /// </summary>
        /// <param name="table">表</param>
        private static void GenerateExcelDataClass(DataTable table)
        {
            // 获取字段名行
            var nameRow = GetVariableNameRow(table);
            // 获取变量类型行
            var typeRow = GetVariableTypeRow(table);

            // 生成数据结构类脚本，就是通过代码进行字符串拼接，然后存进文件
            var dataClassStr = "public class " + table.TableName + "\n" + "{\n";
            for (var i = 0; i < table.Columns.Count; i++)
            {
                dataClassStr += $"\tpublic {typeRow[i]} {nameRow[i]};\n";
            }
            dataClassStr += "}";

            // 判断数据结构类存储文件路径是否存在
            if (!Directory.Exists(DataclassEditorSavePath))
                Directory.CreateDirectory(DataclassEditorSavePath);

            // 保存文件
            File.WriteAllText($"{DataclassEditorSavePath}{table.TableName}.cs", dataClassStr);
            // 刷新窗口
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 生成Excel对应的数据容器类
        /// </summary>
        /// <param name="table">表</param>
        private static void GenerateExcelContainer(DataTable table)
        {
            //获取主键索引
            var keyIndex = GetKeyIndex(table);
            //获取变量类型行
            var typeRow = GetVariableTypeRow(table);

            var dataContainerStr = "using System.Collections.Generic;\n\n";
            dataContainerStr += $"public class {table.TableName}Container\n";
            dataContainerStr += "{\n";
            dataContainerStr += $"\tpublic Dictionary<{typeRow[keyIndex]}, {table.TableName}> dataDic = new Dictionary<{typeRow[keyIndex]}, {table.TableName}>();\n";
            dataContainerStr += "}";

            // 判断数据结构容器类存储文件路径是否存在
            if (!Directory.Exists(DataContainerEditorSavePath))
                Directory.CreateDirectory(DataContainerEditorSavePath);

            // 保存到文件中
            File.WriteAllText(DataContainerEditorSavePath + table.TableName + "Container.cs", dataContainerStr);
            // 刷新窗口
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 生成excel二进制数据
        /// </summary>
        /// <param name="table">表</param>
        private static void GenerateExcelBinary(DataTable table)
        {
            // 路径不存在就创建路径
            if (!Directory.Exists(TableInfoEditorSavePath))
                Directory.CreateDirectory(TableInfoEditorSavePath);

            using (var fs = new FileStream(TableInfoEditorSavePath + table.TableName + ".tInfo.txt", FileMode.OpenOrCreate, FileAccess.Write))
            {
                // 先存储需要写多少行数据，方便读取  -4是因为前面四行是配置规则，不是数据
                fs.Write(BitConverter.GetBytes(table.Rows.Count - BeginIndex), 0, 4);
                // 存储主键变量名
                var keyName = GetVariableNameRow(table)[GetKeyIndex(table)].ToString();
                var keyBytes = Encoding.UTF8.GetBytes(keyName);
                // 存储字符串长度
                fs.Write(BitConverter.GetBytes(keyBytes.Length), 0, 4);
                // 存储字符串
                fs.Write(keyBytes, 0, keyBytes.Length);
                // 得到类型行
                var typeRow = GetVariableTypeRow(table);
                // 遍历所有行
                for (var i = BeginIndex; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    // 遍历所有列
                    for (var j = 0; j < table.Columns.Count; j++)
                    {
                        //根据类型来决定如何写入数据
                        switch (typeRow[j].ToString())
                        {
                            case "int":
                                fs.Write(BitConverter.GetBytes(int.Parse(row[j].ToString())), 0, 4);
                                break;
                            case "float":
                                fs.Write(BitConverter.GetBytes(float.Parse(row[j].ToString())), 0, 4);
                                break;
                            case "bool":
                                fs.Write(BitConverter.GetBytes(bool.Parse(row[j].ToString())), 0, 1);
                                break;
                            case "string":
                                byte[] bytes = Encoding.UTF8.GetBytes(row[j] == null ? "" : row[j].ToString());
                                fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                                fs.Write(bytes, 0, bytes.Length);
                                break;
                        }
                    }
                }
                fs.Close();
            }
            // 刷新窗口
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 获取表中的字段名所在行
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static DataRow GetVariableNameRow(DataTable table)
        {
            return table.Rows[0];
        }

        /// <summary>
        /// 获取表中的字段类型所在行
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static DataRow GetVariableTypeRow(DataTable table)
        {
            return table.Rows[1];
        }

        /// <summary>
        /// 获取主键索引
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static int GetKeyIndex(DataTable table)
        {
            // 获取主键索引所在行
            var typeRow = table.Rows[2];
            for (var i = 0; i < table.Columns.Count; i++)
            {
                if (typeRow[i].ToString() == "key" || typeRow[i].ToString() == "1")
                    return i;
            }
            // 默认以第一个字段作为容器主键
            return 0;
        }
    }
}
