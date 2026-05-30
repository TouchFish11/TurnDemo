using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Core.Extensions;
using Core.Inputs.ActionAsset;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Editor.Generation.Detail
{
    /// <summary>
    /// ��������������
    /// �������ݽṹ�����ݽṹ���������붯��ö��
    /// </summary>
    internal class InputActionDataGenerator : ClassGenerator
    {
        protected override string NameSpace => "Core.Input.ActionAsset";
        protected override string Note { get; set; }
        protected readonly E_AccessModifier accessModifier = E_AccessModifier.Public;
        // ��������
        private readonly string variableType = "string";
        // ��̬���η�
        private readonly string staticModifier = "static";
        // ���ݽṹ�����������ݵ�ӳ��
        private readonly Dictionary<string, string> dataNameToStringMap = new Dictionary<string, string>();

        // ���붯����Դ
        private InputActionAsset inputActions;

        private readonly string filePath = $"{Application.dataPath}/Scripts/Core/Input/ActionAsset/";

        public override void GenerateScript()
        {
            // �������붯����Դ
            inputActions = Resources.Load<InputActionAsset>("PlayerinputAction");

            GenerateInputActionMapEnum();

            GenerateInputActionDataClass();

            GenerateInputActionContainerClass();

            GeneratePlayerActionAssetsJson();
        }

        /// <summary>
        /// �������붯��ӳ��ö��
        /// </summary>
        private void GenerateInputActionMapEnum()
        {
            Dictionary<string, List<string>> mapToActionsMap = new Dictionary<string, List<string>>();

            // �������е�ӳ��
            foreach (InputActionMap map in inputActions.actionMaps)
            {
                mapToActionsMap.Add(map.name, new List<string>());
                // ����ÿ������
                foreach (InputAction action in map.actions)
                {
                    // �Ƿ�����ϰ�
                    bool isComposite = false;
                    // ����ÿ����
                    foreach (var binding in action.bindings)
                    {
                        if (binding.isComposite)
                        {
                            isComposite = true;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(binding.name))
                            {
                                break;
                            }

                            mapToActionsMap[map.name].Add(binding.name.FirstLetterToUpper());
                        }
                    }

                    // �ö���������ϰ󶨣��������Ӷ�����
                    if (!isComposite)
                    {
                        mapToActionsMap[map.name].Add(action.name);
                    }
                }
            }

            foreach (var pair in mapToActionsMap)
            {
                string enumFilePath = $"{filePath}E_{pair.Key}.cs";
                IScriptGenerator scriptGenerator = new InputActionEnumGenerator(pair.Value, new string[] { "None" }, enumFilePath, NameSpace);
                scriptGenerator.GenerateScript();
                Debug.Log($"E_{pair.Key} 生成成功，路径：{enumFilePath}");
            }
        }

        /// <summary>
        /// �������ݽṹ��
        /// </summary>
        private void GenerateInputActionDataClass()
        {
            Dictionary<string, List<InputAction>> mapToActionsMap = new Dictionary<string, List<InputAction>>();

            // �������е�ӳ��
            foreach (InputActionMap map in inputActions.actionMaps)
            {
                mapToActionsMap.Add(map.name, new List<InputAction>());
                // ����ӳ������ж���
                foreach (InputAction action in map.actions)
                {
                    mapToActionsMap[map.name].Add(action);
                }
            }

            foreach (var pair in mapToActionsMap)
            {
                // ���ݽṹ����
                string className = $"{pair.Key}Data";
                List<InputAction> actions = pair.Value;
                Note = $"{className}���붯������";

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"using UnityEngine.InputSystem;");
                sb.AppendLine($"using UnityEngine.InputSystem.LowLevel;");
                sb.AppendLine();
                sb.AppendLine($"namespace {NameSpace}");
                sb.AppendLine("{");
                sb.AppendLine($"\t/// <summary>");
                sb.AppendLine($"\t/// {Note}");
                sb.AppendLine($"\t/// </summary>");
                sb.AppendLine($"\tpublic class {className}");
                sb.AppendLine("\t{");

                foreach (InputAction action in actions)
                {
                    bool isComposite = false;
                    foreach (var inputBinding in action.bindings)
                    {
                        if (inputBinding.isComposite)
                        {
                            isComposite = true;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(inputBinding.name))
                            {
                                break;
                            }

                            // �������ԣ����ݼ���/��겻ͬʹ�ò�ͬ�Ĺ��캯��
                            if (inputBinding.path.Contains("Keyboard"))
                            {
                                string keyStr = inputBinding.path.Split('/')[1];
                                Key key = (Key)Enum.Parse(typeof(Key), keyStr, true);
                                sb.AppendLine($"\t\t[{nameof(ActionKeyMapAttribute)}({nameof(Key)}.{key})]");
                            }
                            else if(inputBinding.path.Contains("Mouse"))
                            {
                                string btnStr = inputBinding.path.Split('/')[1];
                                // ������ͷ�Ϊ��ť��ֵ
                                if (btnStr.Contains("button", StringComparison.OrdinalIgnoreCase))
                                {
                                    string newBtnStr = btnStr.Replace("button", "", StringComparison.OrdinalIgnoreCase);
                                    if (Enum.TryParse<MouseButton>(newBtnStr, true, out var mb))
                                    {
                                        sb.AppendLine($"\t\t[{nameof(ActionKeyMapAttribute)}({nameof(MouseButton)}.{mb})]");
                                    }
                                }
                                else
                                {
                                    if (Enum.TryParse<E_MouseValue>(btnStr, true, out var mv))
                                    {
                                        sb.AppendLine($"\t\t[{nameof(ActionKeyMapAttribute)}({nameof(E_MouseValue)}.{mv})]");
                                    }
                                }
                            }
                            sb.AppendLine($"\t\t{accessModifier.ToEnumString()} {staticModifier} {variableType} {inputBinding.name.FirstLetterToUpper()} => \"{inputBinding.path}\";");
                            sb.AppendLine();
                        }
                    }

                    // �ö�����������ϰ󶨣����Ӷ�����
                    if (!isComposite)
                    {
                        InputBinding inputBinding = action.bindings[0];
                        // �������ԣ����ݼ���/��겻ͬʹ�ò�ͬ�Ĺ��캯��
                        if (inputBinding.path.Contains("Keyboard"))
                        {
                            string keyStr = inputBinding.path.Split('/')[1];
                            Key key = (Key)Enum.Parse(typeof(Key), keyStr, true);
                            sb.AppendLine($"\t\t[{nameof(ActionKeyMapAttribute)}({nameof(Key)}.{key})]");
                        }
                        else if (inputBinding.path.Contains("Mouse"))
                        {
                            string btnStr = inputBinding.path.Split('/')[1];
                            // ������ͷ�Ϊ��ť��ֵ
                            if (btnStr.Contains("button", StringComparison.OrdinalIgnoreCase))
                            {
                                string newBtnStr = btnStr.Replace("button", "", StringComparison.OrdinalIgnoreCase);
                                if (Enum.TryParse<MouseButton>(newBtnStr, true, out var mb))
                                {
                                    sb.AppendLine($"\t\t[{nameof(ActionKeyMapAttribute)}({nameof(MouseButton)}.{mb})]");
                                }
                            }
                            else
                            {
                                if (Enum.TryParse<E_MouseValue>(btnStr, true, out var mv))
                                {
                                    sb.AppendLine($"\t\t[{nameof(ActionKeyMapAttribute)}({nameof(E_MouseValue)}.{mv})]");
                                }
                            }
                        }
                        sb.AppendLine($"\t\t{accessModifier.ToEnumString()} {staticModifier} {variableType} {action.name} => \"{inputBinding.path}\";");
                        sb.AppendLine();
                    }
                }

                sb.AppendLine("\t}");
                sb.AppendLine("}");

                // ����
                dataNameToStringMap.Add(className, sb.ToString());
            }

            // ���ɽű�
            foreach (var pair in dataNameToStringMap)
            {
                string dataFilePath = $"{filePath}{pair.Key}.cs";
                // ��ɾ��������
                if (File.Exists(dataFilePath))
                {
                    File.Delete(dataFilePath);
                }
                File.WriteAllText(dataFilePath, pair.Value);
                Debug.Log($"{pair.Key} 生成成功，路径：{dataFilePath}");
            }

            // ˢ��
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// �������ݽṹ������
        /// </summary>
        private void GenerateInputActionContainerClass()
        {
            Dictionary<string, string> maps = new Dictionary<string, string>();
            foreach (InputActionMap map in inputActions.actionMaps)
            {
                // �ֵ������
                string keyType = $"E_{map.name}";
                // ��������
                string className = $"{map.name}DataContainer";

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("using System;");
                sb.AppendLine($"using System.Collections.Generic;");
                sb.AppendLine();
                sb.AppendLine($"namespace {NameSpace}");
                sb.AppendLine("{");
                sb.AppendLine($"\t/// <summary>");
                sb.AppendLine($"\t/// {Note}数据容器类");
                sb.AppendLine($"\t/// </summary>");
                sb.AppendLine($"\t[Serializable]");
                sb.AppendLine($"\tpublic class {className}");
                sb.AppendLine("\t{");
                sb.AppendLine($"\t\tpublic Dictionary<{keyType}, {nameof(KeyPathMap)}> actionMap = new Dictionary<{keyType}, {nameof(KeyPathMap)}>();");

                sb.AppendLine();
                sb.AppendLine($"\t\tpublic {className}()");
                sb.AppendLine("\t\t{");
                sb.AppendLine($"\t\t\tInputSystem.InitContainer<{map.name}Data>(this);");
                sb.AppendLine("\t\t}");

                sb.AppendLine("\t}");
                sb.AppendLine("}");

                // ���붯��map��ӳ���������ַ���
                maps.Add($"{map.name}", sb.ToString());
            }
        
            foreach (var pair in maps)
            {
                foreach (string dataName in dataNameToStringMap.Keys)
                {
                    // ���ݽṹ����������Ӧ�����붯��ӳ��map���ƣ���Ϊ��Ӧ������
                    if (dataName.Contains(pair.Key))
                    {
                        string containerFilePath = $"{filePath}{dataName}Container.cs";
                        File.WriteAllText(containerFilePath, pair.Value);
                        Debug.Log($"{dataName}Container 生成成功，路径：{containerFilePath}");
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// ����PlayerActionAssets.Json�ļ�
        /// </summary>
        private void GeneratePlayerActionAssetsJson()
        {
            Dictionary<string, string> nameToJsonMap = new Dictionary<string, string>();
            InputActionAsset inputActions = Resources.Load<InputActionAsset>("PlayerinputAction");
            string json = inputActions.ToJson();
            StringBuilder sb = new StringBuilder(json);

            foreach (InputActionMap map in inputActions.actionMaps)
            {
                string className = $"Core.Input.ActionAsset.{map.name}Data";
                Type inputActionDataType = Type.GetType($"{className}, Assembly-CSharp-Core");
                PropertyInfo[] properties = inputActionDataType.GetProperties(BindingFlags.Public | BindingFlags.Static);
                foreach (PropertyInfo propertyInfo in properties)
                {
                    sb.Replace(propertyInfo.GetValue(null).ToString(), $"<{propertyInfo.Name}>");
                }
                nameToJsonMap.Add(map.name, sb.ToString());
            }

            foreach (var item in nameToJsonMap)
            {
                string filePath = $"{Application.dataPath}/Editor/ArtRes/GameConfig/InputConfig/{item.Key}.json";
                File.WriteAllText(filePath, item.Value);
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// ��ȡ2DVector·��
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private int Get2DVectorPath(StringBuilder sb, InputAction action)
        {
            string[] dirs = new string[] { "Up", "Down", "Left", "Right" };

            for (int i = 0; i < action.bindings.Count; i++)
            {
                // ��������
                if (action.bindings[i].path.Contains("Keyboard"))
                {
                    string keyStr = action.bindings[i].path.Split('/')[1];
                    Key key = (Key)Enum.Parse(typeof(Key), keyStr, true);
                    sb.AppendLine($"\t\t[{nameof(ActionKeyMapAttribute)}({nameof(Key)}.{key})]");
                    sb.AppendLine($"\t\t{accessModifier.ToEnumString()} {staticModifier} {variableType} {dirs[i - 1]} => \"{action.bindings[i].path}\";");
                    sb.AppendLine();
                }
            }
            return action.bindings.Count;
        }
    }
}
