using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Core.Editor.Generation.Detail
{
    /// <summary>
    /// AB包键集合类生成器
    /// </summary>
    public class AbKeyCollectionClassGenerator : ClassGenerator
    {
        protected override string Note { get; set; } = "AB包键集合";

        protected override string NameSpace => "HotUpdate.Common";

        private readonly string filePath = $"{Application.dataPath}/Scripts/HotUpdate/Common/{className}.cs";

        private readonly List<string> abNames;
        
        private const string className = "AbKeyCollection";
        
        /// <summary>生成字段的访问修饰符</summary>
        /// <value>默认值：public</value>
        private const string accessModifier = "public";

        /// <summary>生成字段的变量类型</summary>
        /// <value>默认值：string（字符串类型）</value>
        private const string variableType = "string";

        /// <summary>生成字段的静态修饰符</summary>
        /// <value>默认值：static（静态）</value>
        private const string staticModifier = "static";
        
        public AbKeyCollectionClassGenerator(IEnumerable<string> abNames)
        {
            this.abNames = 
            this.abNames = new List<string>(abNames);
        }
        
        public override void GenerateScript()
        {
            // 初始化字符串构建器，初始容量256
            var sb = new StringBuilder(256);

            // 构建命名空间和类的基础结构
            sb.AppendLine($"namespace {NameSpace}");
            sb.AppendLine("{");
            sb.AppendLine("\t/// <summary>");
            sb.AppendLine($"\t/// {Note}");
            sb.AppendLine("\t/// </summary>");
            sb.AppendLine($"\tpublic class {className}");
            sb.AppendLine("\t{");
            
            // 遍历所有扫描到的文件，为每个文件生成静态字符串属性
            foreach (var abName in abNames)
            {
                // 生成一行：public static string 文件名 => "文件名";
                sb.AppendLine($"\t\t{accessModifier} {staticModifier} {variableType} {abName} => \"{abName.ToLower()}\";");
            }

            // 闭合类和命名空间
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            // 若目标文件已存在，先删除（确保覆盖最新内容）
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            
            // 将构建好的代码字符串写入文件
            File.WriteAllText(filePath, sb.ToString());
            // 刷新
            AssetDatabase.Refresh();
        }
    }
}
