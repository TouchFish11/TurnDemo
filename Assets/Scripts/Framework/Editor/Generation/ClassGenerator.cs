using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomEditor.ScriptGeneration
{
    /// <summary>
    /// ÀàÉú³ÉÆ÷
    /// </summary>
    public abstract class ClassGenerator : IScriptGenerator
    {
        public abstract void GenerateScript();
    }
}
