using CustomEditor.ScriptGeneration;
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class InputActionDataClassGenerator : ClassGenerator
{
    public override string FilePath => $"{Application.dataPath}/Scripts/Framework/InputSystem/ActionAsset/InputActionData.cs";

    protected override string NameSpace => "Framework";

    protected override string Note => "输入动作数据";

    protected E_AccessModifier accessModifier = E_AccessModifier.Public;

    // 变量类型
    private readonly string variableType = "string";
    // 静态修饰符
    private readonly string staticModifier = "static";
    // 数据结构名到数据内容的映射
    private readonly Dictionary<string, string> DataNameToStringMap = new Dictionary<string, string>();

    public override void GenerateScript()
    {
        GenerateInputActionDataClass();

        GenerateInputActionContainerClass();
    }


    /// <summary>
    /// 生成数据结构类
    /// </summary>
    private void GenerateInputActionDataClass()
    {
        // 加载输入动作资源
        Dictionary<string, List<InputAction>> mapToActionsMap = new Dictionary<string, List<InputAction>>();
        InputActionAsset inputActions = ResourcesManager.Instance.Load<InputActionAsset>("PlayerinputAction");
        foreach (InputActionMap map in inputActions.actionMaps)
        {
            mapToActionsMap.Add(map.name, new List<InputAction>());
            foreach (InputAction action in map.actions)
            {
                mapToActionsMap[map.name].Add(action);
            }
        }

        foreach (var pair in mapToActionsMap)
        {
            string className = $"{pair.Key}Data";
            List<InputAction> actions = pair.Value;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"using UnityEngine.InputSystem;");
            sb.AppendLine($"using UnityEngine.InputSystem.LowLevel;");
            sb.AppendLine();
            sb.AppendLine($"namespace {NameSpace}");
            sb.AppendLine("{");
            sb.AppendLine($"\t/// <summary>");
            sb.AppendLine($"\t/// {Note}");
            sb.AppendLine($"\t/// <summary>");
            sb.AppendLine($"\tpublic class {className}");
            sb.AppendLine("\t{");

            foreach (InputAction action in actions)
            {
                int num = 0;
                foreach (var inputBinding in action.bindings)
                {
                    if (num != 0)
                    {
                        --num;
                        continue;
                    }

                    if (inputBinding.isComposite)
                    {
                        if (inputBinding.path == "2DVector")
                        {
                            num = Get2DVectorPath(sb, action);
                        }
                    }
                    else
                    {
                        // 添加特性
                        if (inputBinding.path.Contains("Keyboard"))
                        {
                            string keyStr = inputBinding.path.Split('/')[1];
                            Key key = (Key)Enum.Parse(typeof(Key), keyStr, true);
                            sb.AppendLine($"\t\t[{nameof(ActionKeyMapAttribute)}({nameof(Key)}.{key})]");
                        }
                        else if(inputBinding.path.Contains("Mouse"))
                        {
                            string btnStr = inputBinding.path.Split('/')[1];

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
            }

            sb.AppendLine("\t}");
            sb.AppendLine("}");

            // 保存
            DataNameToStringMap.Add(className, sb.ToString());
        }

        // 生成脚本
        foreach (var pair in DataNameToStringMap)
        {
            string filePath = $"{Application.dataPath}/Scripts/Framework/InputSystem/ActionAsset/{pair.Key}.cs";
            // 先删除再生成
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            File.WriteAllText(filePath, pair.Value);
        }

        // 刷新
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 生成数据结构容器类
    /// </summary>
    private void GenerateInputActionContainerClass()
    {
        Dictionary<string, string> maps = new Dictionary<string, string>();
        InputActionAsset inputActions = ResourcesManager.Instance.Load<InputActionAsset>("PlayerinputAction");
        foreach (InputActionMap map in inputActions.actionMaps)
        {
            string keyType = $"E_{map.name}";
            string className = $"{map.name}DataContainer";
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine($"using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine($"namespace {NameSpace}");
            sb.AppendLine("{");
            sb.AppendLine($"\t/// <summary>");
            sb.AppendLine($"\t/// {Note}容器");
            sb.AppendLine($"\t/// <summary>");
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

            maps.Add($"{map.name}", sb.ToString());
        }
        
        foreach (var pair in maps)
        {
            foreach (string dataName in DataNameToStringMap.Keys)
            {
                if (dataName.Contains(pair.Key))
                {
                    string filePath = $"{Application.dataPath}/Scripts/Framework/InputSystem/ActionAsset/{dataName}Container.cs";
                    File.WriteAllText(filePath, pair.Value);
                }
            }
        }

        AssetDatabase.Refresh();
    }


    /// <summary>
    /// 获取2DVector路径
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    private int Get2DVectorPath(StringBuilder sb, InputAction action)
    {
        string[] dirs = new string[] { "Up", "Down", "Left", "Right" };

        for (int i = 0; i < action.bindings.Count; i++)
        {
            // 添加特性
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
