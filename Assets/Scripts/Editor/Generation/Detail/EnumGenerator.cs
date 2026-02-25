using System.Collections.Generic;
using System.IO;
using System.Linq;
using Framework.Editor.Generation;
using UnityEditor;

namespace Editor.Generation.Detail
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
        public string FilePath { get; private set; }

        public EnumGenerator(IEnumerable<string> enumNames, IEnumerable<string> predefinedNames, string filePath, string nameSpace = "")
        {
            this._enumNames = enumNames;
            this._predefinedNames = predefinedNames;
            this.FilePath = filePath;
            this._nameSpace = nameSpace;
            this._enumName = GetEnumName(filePath);
        }

        public void GenerateScript()
        {
            var classStr = "";
            classStr += $"namespace {_nameSpace}\n";
            classStr += "{\n";
            classStr += $"\tpublic enum {_enumName}\n";
            classStr += "\t{\n";
            classStr += _predefinedNames != null ? "\t\t// 预定义类型\n" : "";
            
            if (_predefinedNames != null)
            {
                // 文件夹名称生成枚举类
                // 生成默认枚举项
                foreach (var enumName in _predefinedNames)
                {
                    classStr += $"\t\t{enumName},\n";
                }
            }

            classStr += "\t\t// 生成类型\n";
            // 生成自定义枚举项
            foreach (var abName in _enumNames)
            {
                if (_predefinedNames != null && _predefinedNames.Contains(abName))
                {
                    continue;
                }

                classStr += $"\t\t{abName},\n";
            }

            classStr += "\t}\n";
            classStr += "}";

            // 先删除再生成
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
            File.WriteAllText(FilePath, classStr);

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
