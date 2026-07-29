using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core.Inputs;
using UnityEditor;

namespace Editor.Generation.Detail
{
    /// <summary>
    /// 输入动作枚举生成器
    /// </summary>
    public class InputActionEnumGenerator : IScriptGenerator
    {
        // 命名空间
        private readonly string _nameSpace;
        // 枚举名
        private readonly string _enumName;
        // 枚举成员集合
        private readonly IEnumerable<string> _enumNames;
        // 预定义枚举成员集合
        private readonly IEnumerable<string> _predefinedNames;
        public string FilePath { get; private set; }

        public InputActionEnumGenerator(IEnumerable<string> enumNames, IEnumerable<string> predefinedNames, string filePath, string nameSpace = "")
        {
            this._enumNames = enumNames;
            this._predefinedNames = predefinedNames;
            this.FilePath = filePath;
            this._nameSpace = nameSpace;
            this._enumName = GetEnumName(filePath);
        }

        public void GenerateScript()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine($"namespace {_nameSpace}");
            sb.AppendLine("{");
            sb.AppendLine($"\tpublic enum {_enumName}");
            sb.AppendLine("\t{");
            sb.AppendLine(_predefinedNames != null ? "\t\t// 预定义类型" : "");

            if (_predefinedNames != null)
            {
                // 添加预定义枚举成员
                foreach (string enumName in _predefinedNames)
                {
                    sb.AppendLine($"\t\t{enumName},");
                }
            }

            sb.AppendLine("\t\t// 生成类型");
            // 添加自动生成枚举成员
            foreach (string enumName in _enumNames)
            {
                if (_predefinedNames != null && _predefinedNames.Contains(enumName))
                {
                    continue;
                }
                // 添加特性
                sb.AppendLine($"\t\t[{nameof(ActionMapReplaceKeyAttribute)}(\"<{enumName}>\")]");
                sb.AppendLine($"\t\t{enumName},");
            }

            sb.AppendLine("\t}");
            sb.AppendLine("}");

            // 先删除已存在的文件
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
            File.WriteAllText(FilePath, sb.ToString());

            // 刷新资源数据库
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 获取枚举名
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string GetEnumName(string filePath)
        {
            int index = filePath.LastIndexOf('/');
            string fileName = filePath.Substring(index + 1);
            index = fileName.LastIndexOf('.');
            return fileName.Substring(0, index);
        }
    }
}