using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomEditor.ScriptGeneration
{
    public abstract class ClassGenerator : IScriptGenerator
    {
        public abstract void GenerateScript();
    }
}
