using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomEditor.ScriptGeneration
{
    /// <summary>
    /// 脚本生成接口
    /// </summary>
    public interface IScriptGenerator
    {
        /// <summary>
        /// 生成脚本
        /// </summary>
        void GenerateScript();
    }
}
