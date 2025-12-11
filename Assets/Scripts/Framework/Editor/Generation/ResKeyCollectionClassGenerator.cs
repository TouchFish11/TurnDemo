using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CustomEditor.ScriptGeneration
{
    /// <summary>
    /// 资源键集合类生成器
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
        private readonly string className = "ResConfigCollection";
        // 访问修饰符
        private string accessModifier = "public";
        // 变量类型
        private string variableType = "string";
        // 静态修饰符
        private string staticModifier = "static";
        // 注释
        private string note = "资源键集合类";

        public override void GenerateScript()
        {
            Init();

            string classStr = "";
            classStr += $"namespace {nameSpace}\n";
            classStr += "{\n";
            classStr += "\t/// <summary>\n";
            classStr += $"\t/// {note}\n";
            classStr += "\t/// </summary>\n";
            classStr += $"\tpublic class {className}\n";
            classStr += "\t{\n";

            // 文件夹名称生成类
            foreach (FileInfo fileInfo in fileInfos)
            {
                string name = fileInfo.Name.Substring(0, fileInfo.Name.IndexOf("."));
                classStr += $"\t\t{accessModifier} {staticModifier} {variableType} {name} => \"{name}\";\n";
            }

            classStr += "\t}\n";
            classStr += "}";

            // 先删除再生成
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            File.WriteAllText(filePath, classStr);

            //刷新
            AssetDatabase.Refresh();
        }

        private void Init()
        {
            // 获取根文件夹
            DirectoryInfo directoryInfo = Directory.CreateDirectory(rootPath);
            // 遍历所有的文件夹
            DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();

            for (int i = 0; i < directoryInfos.Length; i++)
            {
                //获取其中一个文件夹下的所有文件
                List<FileInfo> fileInfos = GetTotalFiles(directoryInfos[i], new List<FileInfo>());
                //存储文件列表
                this.fileInfos.AddRange(fileInfos);
            }
        }

        /// <summary>
        /// 获取所有文件
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="fileInfos"></param>
        /// <returns></returns>
        private List<FileInfo> GetTotalFiles(DirectoryInfo directoryInfo, List<FileInfo> fileInfos)
        {
            //获取并存储当前文件夹的所有文件
            List<FileInfo> temps = directoryInfo.GetFiles().ToList();
            for (int i = temps.Count - 1; i >= 0; i--)
            {
                if (_filterSuffixes.Contains(temps[i].Extension))
                {
                    temps.RemoveAt(i);
                }
            }

            fileInfos.AddRange(temps);
            //获取下一级的所有子文件夹
            DirectoryInfo[] subDirectoryInfos = directoryInfo.GetDirectories();
            //存储该级的所有子文件夹信息
            foreach (DirectoryInfo info in subDirectoryInfos)
            {
                GetTotalFiles(info, fileInfos);
            }
            return fileInfos;
        }
    }
}
