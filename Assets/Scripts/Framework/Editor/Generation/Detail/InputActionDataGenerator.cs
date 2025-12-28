using CustomEditor.ScriptGeneration;
using Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

/// <summary>
/// 输入数据生成器
/// 生成数据结构、数据结构容器、输入动作枚举
/// </summary>
public class InputActionDataGenerator : ClassGenerator
{
    protected override string NameSpace => "Framework";
    protected override string Note { get; set; }
    protected E_AccessModifier accessModifier = E_AccessModifier.Public;
    // 变量类型
    private readonly string variableType = "string";
    // 静态修饰符
    private readonly string staticModifier = "static";
    // 数据结构名到数据内容的映射
    private readonly Dictionary<string, string> dataNameToStringMap = new Dictionary<string, string>();

    // 输入动作资源
    private InputActionAsset inputActions;

    private readonly string filePath = $"{Application.dataPath}/Scripts/Framework/InputSystem/ActionAsset/";

    public override void GenerateScript()
    {
        // 加载输入动作资源
        inputActions = ResourcesManager.Instance.Load<InputActionAsset>("PlayerinputAction");

        GenerateInputActionMapEnum();

        GenerateInputActionDataClass();

        GenerateInputActionContainerClass();

        GeneratePlayerActionAssetsJson();
    }

    /// <summary>
    /// 生成输入动作映射枚举
    /// </summary>
    private void GenerateInputActionMapEnum()
    {
        Dictionary<string, List<string>> mapToActionsMap = new Dictionary<string, List<string>>();

        // 遍历所有的映射
        foreach (InputActionMap map in inputActions.actionMaps)
        {
            mapToActionsMap.Add(map.name, new List<string>());
            // 遍历每个动作
            foreach (InputAction action in map.actions)
            {
                // 是否是组合绑定
                bool isComposite = false;
                // 遍历每个绑定
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

                // 该动作存在组合绑定，不用添加动作名
                if (!isComposite)
                {
                    mapToActionsMap[map.name].Add(action.name);
                }
            }
        }

        foreach (var pair in mapToActionsMap)
        {
            string enumFilePath = $"{filePath}E_{pair.Key}.cs";
            IScriptGenerator scriptGenerator = new InputActionEnumGenerator(pair.Value, new string[] { "None" }, enumFilePath, nameof(Framework));
            scriptGenerator.GenerateScript();
            Debug.Log($"E_{pair.Key}，生成路径：{enumFilePath}");
        }
    }

    /// <summary>
    /// 生成数据结构类
    /// </summary>
    private void GenerateInputActionDataClass()
    {
        Dictionary<string, List<InputAction>> mapToActionsMap = new Dictionary<string, List<InputAction>>();

        // 遍历所有的映射
        foreach (InputActionMap map in inputActions.actionMaps)
        {
            mapToActionsMap.Add(map.name, new List<InputAction>());
            // 缓存映射的所有动作
            foreach (InputAction action in map.actions)
            {
                mapToActionsMap[map.name].Add(action);
            }
        }

        foreach (var pair in mapToActionsMap)
        {
            // 数据结构类名
            string className = $"{pair.Key}Data";
            List<InputAction> actions = pair.Value;
            Note = $"{className}输入动作数据";

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

                        // 添加特性，根据键盘/鼠标不同使用不同的构造函数
                        if (inputBinding.path.Contains("Keyboard"))
                        {
                            string keyStr = inputBinding.path.Split('/')[1];
                            Key key = (Key)Enum.Parse(typeof(Key), keyStr, true);
                            sb.AppendLine($"\t\t[{nameof(ActionKeyMapAttribute)}({nameof(Key)}.{key})]");
                        }
                        else if(inputBinding.path.Contains("Mouse"))
                        {
                            string btnStr = inputBinding.path.Split('/')[1];
                            // 鼠标类型分为按钮和值
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

                // 该动作不存在组合绑定，添加动作名
                if (!isComposite)
                {
                    InputBinding inputBinding = action.bindings[0];
                    // 添加特性，根据键盘/鼠标不同使用不同的构造函数
                    if (inputBinding.path.Contains("Keyboard"))
                    {
                        string keyStr = inputBinding.path.Split('/')[1];
                        Key key = (Key)Enum.Parse(typeof(Key), keyStr, true);
                        sb.AppendLine($"\t\t[{nameof(ActionKeyMapAttribute)}({nameof(Key)}.{key})]");
                    }
                    else if (inputBinding.path.Contains("Mouse"))
                    {
                        string btnStr = inputBinding.path.Split('/')[1];
                        // 鼠标类型分为按钮和值
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

            // 保存
            dataNameToStringMap.Add(className, sb.ToString());
        }

        // 生成脚本
        foreach (var pair in dataNameToStringMap)
        {
            string dataFilePath = $"{filePath}{pair.Key}.cs";
            // 先删除再生成
            if (File.Exists(dataFilePath))
            {
                File.Delete(dataFilePath);
            }
            File.WriteAllText(dataFilePath, pair.Value);
            Debug.Log($"{pair.Key}，生成路径：{dataFilePath}");
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
        foreach (InputActionMap map in inputActions.actionMaps)
        {
            // 字典键类型
            string keyType = $"E_{map.name}";
            // 容器类名
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

            // 输入动作map名映射容器类字符串
            maps.Add($"{map.name}", sb.ToString());
        }
        
        foreach (var pair in maps)
        {
            foreach (string dataName in dataNameToStringMap.Keys)
            {
                // 数据结构类名包含对应的输入动作映射map名称，则为对应容器类
                if (dataName.Contains(pair.Key))
                {
                    string containerFilePath = $"{filePath}{dataName}Container.cs";
                    File.WriteAllText(containerFilePath, pair.Value);
                    Debug.Log($"{dataName}Container，生成路径：{containerFilePath}");
                }
            }
        }

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 生成PlayerActionAssets.Json文件
    /// </summary>
    private void GeneratePlayerActionAssetsJson()
    {
        Dictionary<string, string> nameToJsonMap = new Dictionary<string, string>();
        InputActionAsset inputActions = ResourcesManager.Instance.Load<InputActionAsset>("PlayerinputAction");
        string json = inputActions.ToJson();
        StringBuilder sb = new StringBuilder(json);

        foreach (InputActionMap map in inputActions.actionMaps)
        {
            string className = $"{nameof(Framework)}.{map.name}Data";
            Type inputActionDataType = Type.GetType($"{className}, Assembly-CSharp");
            PropertyInfo[] properties = inputActionDataType.GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (PropertyInfo propertyInfo in properties)
            {
                sb.Replace(propertyInfo.GetValue(null).ToString(), $"<{propertyInfo.Name}>");
            }
            nameToJsonMap.Add(map.name, sb.ToString());
        }

        foreach (var item in nameToJsonMap)
        {
            string filePath = $"{Application.dataPath}/Editor/ArtRes/GameData/Input/{item.Key}.json";
            File.WriteAllText(filePath, item.Value);
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
