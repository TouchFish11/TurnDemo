using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace CustomEditor.ScriptGeneration
{
    /// <summary>
    /// 枚举生成器
    /// </summary>
    public class EnumGenerator : IScriptGenerator
    {
        // 命名空间
        private readonly string _nameSpace;
        // 枚举名
        private readonly string _enumName;
        // 枚举项名
        private readonly IEnumerable<string> _enumNames;
        // 预定义枚举项名
        private readonly IEnumerable<string> _predefinedNames;
        // 文件保存路径
        private readonly string _filePath;

        public EnumGenerator(IEnumerable<string> enumNames, IEnumerable<string> predefinedNames, string filePath, string nameSpace = "")
        {
            this._enumNames = enumNames;
            this._predefinedNames = predefinedNames;
            this._filePath = filePath;
            this._nameSpace = nameSpace;
            this._enumName = GetEnumName(filePath);
        }

        public void GenerateScript()
        {
            string classStr = "";
            classStr += $"namespace {_nameSpace}\n";
            classStr += "{\n";
            classStr += $"\tpublic enum {_enumName}\n";
            classStr += "\t{\n";
            classStr += "\t\t// 预定义类型\n";

            // 文件夹名称生成枚举类
            // 生成默认枚举项
            foreach (string enumName in _predefinedNames)
            {
                classStr += $"\t\t{enumName},\n";
            }

            classStr += "\t\t// 生成类型\n";

            // 生成自定义枚举项
            foreach (string abName in _enumNames)
            {
                if (_predefinedNames.Contains(abName))
                {
                    continue;
                }

                classStr += $"\t\t{abName},\n";
            }

            classStr += "\t}\n";
            classStr += "}";

            // 先删除再生成
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
            File.WriteAllText(_filePath, classStr);

            //刷新
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
