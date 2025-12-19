using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CustomEditor.ScriptGeneration
{
    /// <summary>
    /// 资源配置集合类生成器
    /// </summary>
    public class ResKeyCollectionClassGenerator : ClassGenerator
    {
        private readonly string rootPath = $"{Application.dataPath}/Editor/ArtRes";
        // 类文件生成路径
        private readonly string filePath = Application.dataPath + "/Scripts/Framework/Config/ResKeyCollection.cs";
        // 文件过滤后缀数组
        private readonly string[] _filterSuffixes = new string[] { ".meta" };
        // 文件列表
        private readonly List<FileInfo> fileInfos = new List<FileInfo>();
        // 命名空间
        private readonly string nameSpace = "Framework";
        // 类名
        private readonly string className = "ResKeyCollection";
        // 访问修饰符
        private readonly string accessModifier = "public";
        // 变量类型
        private readonly string variableType = "string";
        // 静态修饰符
        private readonly string staticModifier = "static";
        // 注释
        private readonly string note = "资源键集合类";

        private void Init()
        {
            // 获取根文件夹
            DirectoryInfo directoryInfo = Directory.CreateDirectory(rootPath);
            // 遍历所有的文件夹
            DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();

            for (int i = 0; i < directoryInfos.Length; i++)
            {
                //获取其中一个文件夹下的所有文件
                List<FileInfo> fileInfos = FileUtility.GetTotalFiles(directoryInfos[i], new List<FileInfo>(), _filterSuffixes);
                //存储文件列表
                this.fileInfos.AddRange(fileInfos);
            }
        }

        public override void GenerateScript()
        {
            Init();

            StringBuilder sb = new StringBuilder(256);
            sb.AppendLine($"namespace {nameSpace}");
            sb.AppendLine("{");
            sb.AppendLine($"\t/// <summary>");
            sb.AppendLine($"\t/// {note}");
            sb.AppendLine($"\t/// <summary>");
            sb.AppendLine($"\tpublic class {className}");
            sb.AppendLine("\t{");

            // 文件夹名称生成类
            foreach (FileInfo fileInfo in fileInfos)
            {
                string name = fileInfo.Name.Substring(0, fileInfo.Name.IndexOf("."));

                sb.AppendLine($"\t\t{accessModifier} {staticModifier} {variableType} {name} => \"{name}\";");
            }

            sb.AppendLine("\t}");
            sb.AppendLine("}");

            // 先删除再生成
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            File.WriteAllText(filePath, sb.ToString());

            //刷新
            AssetDatabase.Refresh();
        }
    }
}
