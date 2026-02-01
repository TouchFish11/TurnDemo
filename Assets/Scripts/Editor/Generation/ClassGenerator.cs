using System.Collections;
using System.Collections.Generic;
using Framework.Editor.Generation;
using UnityEngine;

namespace CustomEditor.ScriptGeneration
{
    /// <summary>
    /// ��������
    /// </summary>
    public abstract class ClassGenerator : IScriptGenerator
    {
        // ע��
        protected abstract string Note { get; set; }
        // �����ռ�
        protected abstract string NameSpace { get; }
        //// ���ļ�����·��
        //public abstract string filePath { get; }

        public abstract void GenerateScript();
    }
}
