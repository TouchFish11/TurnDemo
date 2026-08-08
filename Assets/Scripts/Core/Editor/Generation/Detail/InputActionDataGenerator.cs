using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Core.Extensions;
using Core.Inputs;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Core.Editor.Generation.Detail
{
    /// <summary>
    /// 输入动作数据生成器
    /// 生成数据结构、数据容器和输入动作枚举
    /// </summary>
    internal class InputActionDataGenerator : ClassGenerator
    {
        protected override string NameSpace => "Core.Inputs";
        protected override string Note { get; set; }
        protected readonly E_AccessModifier accessModifier = E_AccessModifier.Public;
        // 变量类型
        private readonly string variableType = "string";
        // 静态修饰符
        private readonly string staticModifier = "static";
        // 数据结构名称到生成数据的映射
        private readonly Dictionary<string, string> dataNameToStringMap = new();

        // 输入动作资源
        private InputActionAsset inputActions;

        private readonly string filePath = $"{Application.dataPath}/Scripts/Core/Input/ActionAsset/";

        public override void GenerateScript()
        {
            // 加载输入动作资源
            inputActions = Resources.Load<InputActionAsset>("PlayerInputAction");

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
            var mapToActionsMap = new Dictionary<string, List<string>>();

            // 遍历所有的映射
            foreach (var map in inputActions.actionMaps)
            {
                mapToActionsMap.Add(map.name, new List<string>());
                // 遍历每个动作
                foreach (var action in map.actions)
                {
                    // 是否是组合绑定
                    var isComposite = false;
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

                    // 该动作没有组合绑定，直接添加动作名称
                    if (!isComposite)
                    {
                        mapToActionsMap[map.name].Add(action.name);
                    }
                }
            }

            foreach (var pair in mapToActionsMap)
            {
                var enumFilePath = $"{filePath}E_{pair.Key}.cs";
                IScriptGenerator scriptGenerator = new InputActionEnumGenerator(pair.Value, new[] { "None" }, enumFilePath, NameSpace);
                scriptGenerator.GenerateScript();
                Debug.Log($"E_{pair.Key} 生成成功，路径：{enumFilePath}");
            }
        }

        /// <summary>
        /// 生成数据结构类
        /// </summary>
        private void GenerateInputActionDataClass()
        {
            var mapToActionsMap = new Dictionary<string, List<InputAction>>();

            // 遍历所有的映射
            foreach (var map in inputActions.actionMaps)
            {
                mapToActionsMap.Add(map.name, new List<InputAction>());
                // 遍历映射中所有动作
                foreach (var action in map.actions)
                {
                    mapToActionsMap[map.name].Add(action);
                }
            }

            foreach (var pair in mapToActionsMap)
            {
                // 数据结构类名
                var className = $"{pair.Key}Data";
                var actions = pair.Value;
                Note = $"{className}输入动作数据";

                var sb = new StringBuilder();
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

                foreach (var action in actions)
                {
                    var isComposite = false;
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

                            // 添加属性，根据键盘/鼠标不同使用不同的构造函数
                            if (inputBinding.path.Contains("Keyboard"))
                            {
                                var keyStr = inputBinding.path.Split('/')[1];
                                var key = (Key)Enum.Parse(typeof(Key), keyStr, true);
                                sb.AppendLine($"\t\t[{nameof(ActionKeyMapAttribute)}({nameof(Key)}.{key})]");
                            }
                            else if(inputBinding.path.Contains("Mouse"))
                            {
                                var btnStr = inputBinding.path.Split('/')[1];
                                // 将鼠标输入分为按键和数值
                                if (btnStr.Contains("button", StringComparison.OrdinalIgnoreCase))
                                {
                                    var newBtnStr = btnStr.Replace("button", "", StringComparison.OrdinalIgnoreCase);
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

                    // 该动作没有组合绑定，直接添加动作
                    if (!isComposite)
                    {
                        var inputBinding = action.bindings[0];
                        // 添加属性，根据键盘/鼠标不同使用不同的构造函数
                        if (inputBinding.path.Contains("Keyboard"))
                        {
                            var keyStr = inputBinding.path.Split('/')[1];
                            var key = (Key)Enum.Parse(typeof(Key), keyStr, true);
                            sb.AppendLine($"\t\t[{nameof(ActionKeyMapAttribute)}({nameof(Key)}.{key})]");
                        }
                        else if (inputBinding.path.Contains("Mouse"))
                        {
                            var btnStr = inputBinding.path.Split('/')[1];
                            // 将鼠标输入分为按键和数值
                            if (btnStr.Contains("button", StringComparison.OrdinalIgnoreCase))
                            {
                                var newBtnStr = btnStr.Replace("button", "", StringComparison.OrdinalIgnoreCase);
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

                // 收集
                dataNameToStringMap.Add(className, sb.ToString());
            }

            // 生成脚本
            foreach (var pair in dataNameToStringMap)
            {
                var dataFilePath = $"{filePath}{pair.Key}.cs";
                // 先删除已存在的文件
                if (File.Exists(dataFilePath))
                {
                    File.Delete(dataFilePath);
                }
                File.WriteAllText(dataFilePath, pair.Value);
                Debug.Log($"{pair.Key} 生成成功，路径：{dataFilePath}");
            }

            // 刷新
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 生成数据结构容器类
        /// </summary>
        private void GenerateInputActionContainerClass()
        {
            var maps = new Dictionary<string, string>();
            foreach (var map in inputActions.actionMaps)
            {
                // 键类型（枚举）
                var keyType = $"E_{map.name}";
                // 类名
                var className = $"{map.name}DataContainer";

                var sb = new StringBuilder();
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

                // 输入动作map的映射容器和生成字符串
                maps.Add($"{map.name}", sb.ToString());
            }
        
            foreach (var pair in maps)
            {
                foreach (var dataName in dataNameToStringMap.Keys)
                {
                    // 数据结构名称包含对应的输入动作映射map名称，即为对应容器
                    if (dataName.Contains(pair.Key))
                    {
                        var containerFilePath = $"{filePath}{dataName}Container.cs";
                        File.WriteAllText(containerFilePath, pair.Value);
                        Debug.Log($"{dataName}Container 生成成功，路径：{containerFilePath}");
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
            var nameToJsonMap = new Dictionary<string, string>();
            var inputActions = Resources.Load<InputActionAsset>("PlayerInputAction");
            var json = inputActions.ToJson();
            var sb = new StringBuilder(json);

            foreach (var map in inputActions.actionMaps)
            {
                var className = $"Core.Input.ActionAsset.{map.name}Data";
                var inputActionDataType = Type.GetType($"{className}, Assembly-CSharp-Core");
                var properties = inputActionDataType.GetProperties(BindingFlags.Public | BindingFlags.Static);
                foreach (var propertyInfo in properties)
                {
                    sb.Replace(propertyInfo.GetValue(null).ToString(), $"<{propertyInfo.Name}>");
                }
                nameToJsonMap.Add(map.name, sb.ToString());
            }

            foreach (var item in nameToJsonMap)
            {
                var filePath = $"{Application.dataPath}/Editor/ArtRes/GameConfig/InputConfig/{item.Key}.json";
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
            var dirs = new[] { "Up", "Down", "Left", "Right" };

            for (var i = 0; i < action.bindings.Count; i++)
            {
                // 遍历绑定
                if (action.bindings[i].path.Contains("Keyboard"))
                {
                    var keyStr = action.bindings[i].path.Split('/')[1];
                    var key = (Key)Enum.Parse(typeof(Key), keyStr, true);
                    sb.AppendLine($"\t\t[{nameof(ActionKeyMapAttribute)}({nameof(Key)}.{key})]");
                    sb.AppendLine($"\t\t{accessModifier.ToEnumString()} {staticModifier} {variableType} {dirs[i - 1]} => \"{action.bindings[i].path}\";");
                    sb.AppendLine();
                }
            }
            return action.bindings.Count;
        }
    }
}