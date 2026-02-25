using System.Collections.Generic;
using System.IO;
using System.Text;
using Core.Utility;
using UnityEditor;
using UnityEngine;

namespace Editor.Generation.Detail
{
    /// <summary>
    /// 访问修饰符枚举
    /// 定义代码生成时可使用的类/字段访问修饰符类型
    /// </summary>
    public enum E_AccessModifier
    {
        /// <summary>无修饰符</summary>
        None,
        /// <summary>公共访问修饰符</summary>
        Public,
        /// <summary>受保护访问修饰符</summary>
        Protected,
        /// <summary>私有访问修饰符</summary>
        Private,
        /// <summary>内部访问修饰符</summary>
        Internal,
    }

    /// <summary>
    /// 访问修饰符枚举的扩展方法类
    /// 提供将枚举值转换为小写字符串的功能
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 将访问修饰符枚举转换为对应的小写字符串
        /// 示例：E_AccessModifier.Public → "public"
        /// </summary>
        /// <param name="e_AccessModifier">要转换的访问修饰符枚举值</param>
        /// <returns>小写的修饰符字符串</returns>
        public static string ToEnumString(this E_AccessModifier e_AccessModifier)
        {
            return e_AccessModifier.ToString().ToLower();
        }
    }

    /// <summary>
    /// 资源Key集合类生成器
    /// 作用：扫描指定目录下的资源文件，自动生成包含资源名称常量的ResKeyCollection类
    /// 继承自ClassGenerator（自定义的代码生成基类）
    /// </summary>
    public class ResKeyCollectionClassGenerator : ClassGenerator
    {
        /// <summary>资源文件根目录路径</summary>
        /// <value>默认指向Unity工程的 Assets/Editor/ArtRes 目录</value>
        private readonly string rootPath = $"{Application.dataPath}/Editor/ArtRes";

        /// <summary>需要过滤的文件后缀名数组</summary>
        /// <value>默认过滤.meta文件（Unity的元数据文件）</value>
        private readonly string[] _filterSuffixes = { ".meta", ".bytes" };
        
        /// <summary>存储扫描到的有效文件信息列表</summary>
        private readonly List<FileInfo> fileInfos = new List<FileInfo>();

        /// <summary>生成的类名称</summary>
        /// <value>默认值：ResKeyCollection</value>
        private const string className = "ResKeyCollection";

        /// <summary>生成字段的访问修饰符</summary>
        /// <value>默认值：public</value>
        private const string accessModifier = "public";

        /// <summary>生成字段的变量类型</summary>
        /// <value>默认值：string（字符串类型）</value>
        private const string variableType = "string";

        /// <summary>生成字段的静态修饰符</summary>
        /// <value>默认值：static（静态）</value>
        private const string staticModifier = "static";

        /// <summary>生成的C#脚本文件保存路径</summary>
        private readonly string filePath = $"{Application.dataPath}/Scripts/GameHotUpdate/Config/ResKeyCollection.cs";
        
        /// <summary>生成类的命名空间</summary>
        /// <value>固定为Core命名空间</value>
        protected override string NameSpace => "GameHotUpdate.Config";
        
        /// <summary>生成类的注释描述</summary>
        protected override string Note { get; set; }

        /// <summary>初始化方法</summary>
        /// <remarks>
        /// 1. 创建（确保）资源根目录存在
        /// 2. 扫描根目录下所有子目录
        /// 3. 递归获取所有非过滤后缀的文件信息并存储
        /// </remarks>
        private void Init()
        {
            // 创建目录（若不存在则创建，存在则返回现有目录信息）
            var directoryInfo = Directory.CreateDirectory(rootPath);
            // 获取根目录下的所有子目录信息
            var directoryInfos = directoryInfo.GetDirectories();

            // 遍历所有子目录
            foreach (var info in directoryInfos)
            {
                // 递归获取当前子目录下所有非过滤后缀的文件列表
                var totalFiles = FileUtility.GetTotalFiles(info, new List<FileInfo>(), _filterSuffixes);
                // 将当前目录的文件信息添加到全局文件列表中
                this.fileInfos.AddRange(totalFiles);
            }
        }

        /// <summary>重写基类的脚本生成方法</summary>
        /// <remarks>
        /// 核心流程：
        /// 1. 初始化（扫描文件）
        /// 2. 构建类的代码字符串
        /// 3. 写入并覆盖目标文件
        /// 4. 刷新Unity资源数据库
        /// </remarks>
        public override void GenerateScript()
        {
            // 初始化：扫描目标目录下的文件
            Init();

            // 初始化字符串构建器，初始容量256（减少内存扩容）
            var sb = new StringBuilder(256);
            // 设置类的注释描述
            Note = "资源键值集合类，自动生成，包含所有资源名称的静态字符串常量";

            // 构建命名空间和类的基础结构
            sb.AppendLine($"namespace {NameSpace}");
            sb.AppendLine("{");
            sb.AppendLine("\t/// <summary>");
            sb.AppendLine($"\t/// {Note}");
            sb.AppendLine("\t/// </summary>");
            sb.AppendLine($"\tpublic class {className}");
            sb.AppendLine("\t{");

            // 遍历所有扫描到的文件，为每个文件生成静态字符串属性
            foreach (FileInfo fileInfo in fileInfos)
            {
                // 获取文件名（去除后缀）作为字段名和属性值
                string name = fileInfo.Name.Substring(0, fileInfo.Name.IndexOf("."));

                // 生成一行：public static string 文件名 => "文件名";
                // 使用表达式体属性简化写法，避免字段赋值的冗余代码
                sb.AppendLine($"\t\t{accessModifier} {staticModifier} {variableType} {name} => \"{name}\";");
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

            // 刷新Unity资源数据库，使生成的脚本在编辑器中立即可见
            AssetDatabase.Refresh();
        }
    }
}