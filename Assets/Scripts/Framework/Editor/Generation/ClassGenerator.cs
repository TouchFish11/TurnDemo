using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomEditor.ScriptGeneration
{
    /// <summary>
    /// 类生成器
    /// </summary>
    public abstract class ClassGenerator : IScriptGenerator
    {
        // 注释
        protected abstract string Note { get; set; }
        // 命名空间
        protected abstract string NameSpace { get; }
        //// 类文件生成路径
        //public abstract string filePath { get; }

        public abstract void GenerateScript();
    }
}
