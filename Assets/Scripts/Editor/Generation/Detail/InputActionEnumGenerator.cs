using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core.Input.ActionAsset;
using Framework.Editor.Generation;
using UnityEditor;

namespace Editor.Generation.Detail
{
    /// <summary>
    /// ���붯��ö��������
    /// </summary>
    public class InputActionEnumGenerator : IScriptGenerator
    {
        // �����ռ�
        private readonly string _nameSpace;
        // ö����
        private readonly string _enumName;
        // ö������
        private readonly IEnumerable<string> _enumNames;
        // Ԥ����ö������
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
                // �ļ�����������ö����
                // ����Ĭ��ö����
                foreach (string enumName in _predefinedNames)
                {
                    sb.AppendLine($"\t\t{enumName},");
                }
            }

            sb.AppendLine("\t\t// 生成类型");
            // �����Զ���ö����
            foreach (string enumName in _enumNames)
            {
                if (_predefinedNames != null && _predefinedNames.Contains(enumName))
                {
                    continue;
                }
                // ��������
                sb.AppendLine($"\t\t[{nameof(ActionMapReplaceKeyAttribute)}(\"<{enumName}>\")]");
                sb.AppendLine($"\t\t{enumName},");
            }

            sb.AppendLine("\t}");
            sb.AppendLine("}");

            // ��ɾ��������
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
            File.WriteAllText(FilePath, sb.ToString());

            //ˢ��
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// ��ȡö����
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
