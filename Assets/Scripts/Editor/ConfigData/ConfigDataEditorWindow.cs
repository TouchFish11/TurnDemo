using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Editor.Generation.Detail;
using Framework.Editor.Generation;
using UnityEditor;
using UnityEngine;

namespace Editor.ConfigData
{
    /// <summary>
    /// 配置数据编辑窗口
    /// 用于可视化编辑配置数据结构、字段及具体数据，并支持代码生成、数据保存/加载
    /// </summary>
    public class ConfigDataEditorWindow : EditorWindow
    {
        // 字段索引（用于生成新字段名的自增序号）
        private static uint fieldIndex = 1;

        // 选中的配置数据对象
        private ConfigData selectConfigData;
        // 新建配置的名称
        private string configName = "NewInfo";
        // 当前选中的行索引
        private int _selectedRowIndex = -1;

        // 滚动位置（列模板区域）
        private Vector2 _columnScrollPos;
        // 滚动位置（数据表格区域）
        private Vector2 _tableScrollPos;

        // 临时编辑数据（新字段名称）
        private readonly string _newFieldName = $"newField";
        // 临时编辑数据（新字段类型）
        private E_FieldType _newFieldType = E_FieldType.None;

        // 配置数据列表（存储所有加载的配置）
        private readonly List<ConfigData> configDatas = new();

        // 搜索文本（筛选配置用）
        private string searchText;
        // 配置列表滚动位置
        private Vector2 scrollPosition;
        // 配置文件保存路径（相对Assets目录）
        private string configSavePath = $"Editor/ConfigData/";

        // 代码生成器接口实例
        private IScriptGenerator scriptGenerator;

        /// <summary>
        /// 打开配置数据编辑窗口（Unity编辑器菜单入口）
        /// </summary>
        [MenuItem("GameTool/EditorWindow/Config Data Editor")]
        public static void OpenWindow()
        {
            var window = GetWindow<ConfigDataEditorWindow>("Config Data Editor");
            window.minSize = new Vector2(900, 700);
            fieldIndex = 1;
            window.LoadConfigs(); // 初始化加载所有配置
        }

        #region UI绘制区域
        /// <summary>
        /// 窗口GUI绘制主入口
        /// </summary>
        private void OnGUI()
        {
            ToolbarArea();
            EditorGUILayout.BeginHorizontal();

            DrawConfigDatasArea();
            DrawFieldTempleteArea();

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 窗口启用时触发（注册程序集重载事件）
        /// </summary>
        private void OnEnable()
        {
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        /// <summary>
        /// 窗口禁用时触发（注销程序集重载事件）
        /// </summary>
        private void OnDisable()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }

        /// <summary>
        /// 程序集重载前执行（保存所有配置）
        /// </summary>
        private void OnBeforeAssemblyReload()
        {
            SaveConfigs();
            Debug.Log("程序集重载前已保存所有配置数据");
        }

        /// <summary>
        /// 程序集重载后执行（重新加载配置并刷新界面）
        /// </summary>
        private void OnAfterAssemblyReload()
        {
            selectConfigData = null;
            LoadConfigs(); // 从本地文件重新加载配置
            Repaint(); // 刷新窗口绘制
            Debug.Log("程序集重载后已重新加载配置");
        }

        /// <summary>
        /// 绘制顶部工具栏区域
        /// 包含：搜索框、保存路径配置、保存按钮
        /// </summary>
        private void ToolbarArea()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            // 搜索框
            searchText = EditorGUILayout.TextField(searchText, EditorStyles.toolbarSearchField, GUILayout.Width(300));
            
            // 配置保存路径标签+输入框
            GUILayout.Label(new GUIContent("ConfigDataSavePath", "配置文件的保存路径（相对Assets）"), EditorStyles.boldLabel, GUILayout.Width(130));
            configSavePath = EditorGUILayout.TextField(configSavePath, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));

            // 保存选中配置按钮
            if (GUILayout.Button("保存当前选中配置", GUILayout.Width(200)))
            {
                SaveConfig();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制配置数据列表区域（左侧面板）
        /// 包含：配置列表、新建配置输入框+按钮
        /// </summary>
        private void DrawConfigDatasArea()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(300));
            GUILayout.Label("配置数据列表", EditorStyles.boldLabel);
            // 绘制配置列表
            DrawConfigDatas();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制配置数据列表（核心逻辑）
        /// 支持搜索筛选、配置选中、配置删除
        /// </summary>
        private void DrawConfigDatas()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            var isDel = false;
            foreach (var configData in configDatas)
            {
                // 搜索筛选逻辑
                if (string.IsNullOrEmpty(searchText) || configData.configName.Contains(searchText))
                {
                    EditorGUILayout.BeginHorizontal();

                    // 配置选中按钮
                    if (GUILayout.Button(configData.configName, EditorStyles.miniButtonLeft))
                    {
                        selectConfigData = configData;
                    }

                    // 配置删除按钮
                    if (GUILayout.Button("Delete", EditorStyles.miniButtonRight, GUILayout.Width(60)))
                    {
                        if (EditorUtility.DisplayDialog("提示", $"确定要删除配置 '{configData.configName}' 吗？", "是", "否"))
                        {
                            isDel = true;
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (!isDel)
                {
                    continue;
                }
                
                configDatas.Remove(configData);
                break;
            }

            EditorGUILayout.EndScrollView();

            // 空行分隔
            EditorGUILayout.Space();

            // 新建配置名称输入框
            configName = EditorGUILayout.TextField("配置名称", configName);
            GUILayout.Space(10);

            // 新建配置按钮
            if (GUILayout.Button("创建新配置"))
            {
                CreateConfigData();
            }
        }

        /// <summary>
        /// 创建新配置数据
        /// 包含路径检查、重名校验、文件序列化保存
        /// </summary>
        private void CreateConfigData()
        {
            // 空名称校验
            if (string.IsNullOrEmpty(configName))
            {
                EditorUtility.DisplayDialog("提示", "配置名称不能为空", "确定");
                return;
            }

            // 保存路径检查（不存在则创建）
            if (!Directory.Exists(GetSavePath()))
            {
                Directory.CreateDirectory(GetSavePath());
            }

            // 重名校验
            var assetPath = $"{GetSavePath()}{configName}.json";
            if (File.Exists(assetPath))
            {
                EditorUtility.DisplayDialog("提示", $"已存在名为 '{configName}' 的配置文件", "确定");
                return;
            }

            // 创建新配置对象
            var newConfigData = new ConfigData(configName);
            // 序列化保存到文件
            var json = JsonUtility.ToJson(newConfigData, true);
            File.WriteAllText(assetPath, json);
            // 添加到内存列表
            configDatas.Add(newConfigData);
            AssetDatabase.Refresh();
            Debug.Log($"配置创建成功，路径:{assetPath}");
        }

        /// <summary>
        /// 绘制字段模板+数据编辑区域（右侧面板）
        /// 包含：列模板编辑、数据表格编辑、底部操作按钮
        /// </summary>
        private void DrawFieldTempleteArea()
        {
            EditorGUILayout.BeginVertical();

            // 列模板编辑区域
            DrawColumnTemplateArea();
            GUILayout.Space(10);

            // 数据表格编辑区域
            DrawDataEditorArea();
            GUILayout.Space(10);

            // 底部操作按钮
            DrawBottomButtons();

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制列模板编辑区域
        /// 支持字段编辑、字段删除、新增字段、代码生成
        /// </summary>
        private void DrawColumnTemplateArea()
        {
            EditorGUILayout.BeginVertical("box");

            GUILayout.Label("字段模板编辑", EditorStyles.boldLabel);

            _columnScrollPos = EditorGUILayout.BeginScrollView(_columnScrollPos, GUILayout.Height(150));

            // 列模板表头
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("主键", GUILayout.Width(50));
            GUILayout.Label("字段名称", GUILayout.Width(250));
            GUILayout.Label("字段类型", GUILayout.Width(100));
            GUILayout.Label("字段描述", GUILayout.ExpandWidth(true));
            GUILayout.Label("操作", GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            if (selectConfigData != null)
            {
                var isDelete = false;
                // 遍历列模板列表
                for (var i = 0; i < selectConfigData.columns.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    // 主键勾选框
                    selectConfigData.columns[i].key = EditorGUILayout.Toggle(selectConfigData.columns[i].key, GUILayout.Width(50));

                    // 字段名称编辑（同步更新行数据中的字段名）
                    var oldFieldName = selectConfigData.columns[i].fieldName;
                    selectConfigData.columns[i].fieldName = EditorGUILayout.TextField(selectConfigData.columns[i].fieldName, GUILayout.Width(250));
                    if (selectConfigData.columns[i].fieldName != oldFieldName)
                    {
                        foreach (var rowData in selectConfigData.rows)
                        {
                            var oldValue = rowData[oldFieldName];
                            rowData.Remove(oldFieldName);
                            if (!rowData.TryAdd(selectConfigData.columns[i].fieldName, oldValue))
                            {
                                EditorUtility.DisplayDialog("错误", $"字段名称{selectConfigData.columns[i].fieldName}已存在", "确定");
                                selectConfigData.columns[i].fieldName = oldFieldName;
                                rowData.TryAdd(oldFieldName, oldValue);
                            }
                        }
                    }

                    // 字段类型编辑（同步重置行数据中的字段值）
                    var oldType = selectConfigData.columns[i].fieldType;
                    selectConfigData.columns[i].fieldType = (E_FieldType)EditorGUILayout.EnumPopup(selectConfigData.columns[i].fieldType, GUILayout.Width(100));
                    if (selectConfigData.columns[i].fieldType != oldType)
                    {
                        foreach (var rowData in selectConfigData.rows)
                        {
                            var value = string.Empty;
                            switch (selectConfigData.columns[i].fieldType)
                            {
                                case E_FieldType.Int:
                                    value = 0.ToString();
                                    break;
                                case E_FieldType.Float:
                                    value = 0.0f.ToString();
                                    break;
                                case E_FieldType.Bool:
                                    value = false.ToString();
                                    break;
                            }
                            rowData.SetValue(selectConfigData.columns[i].fieldName, value);
                        }
                    }

                    // 字段描述编辑
                    selectConfigData.columns[i].fieldDescription = EditorGUILayout.TextField(selectConfigData.columns[i].fieldDescription, GUILayout.ExpandWidth(true));
                    
                    // 字段删除按钮
                    if (GUILayout.Button("×", GUILayout.Width(50)))
                    {
                        // 同步删除行数据中的对应字段
                        foreach (var row in selectConfigData.rows)
                        {
                            row.Remove(selectConfigData.columns[i].fieldName);
                        }
                        selectConfigData.columns.RemoveAt(i);
                        isDelete = true;
                    }
                    
                    EditorGUILayout.EndHorizontal();

                    if (isDelete)
                    {
                        break;
                    }
                }
            }

            EditorGUILayout.EndScrollView();

            // 列模板操作按钮
            EditorGUILayout.BeginHorizontal();

            // 新增字段按钮
            if (GUILayout.Button("添加新字段", GUILayout.Width(200)))
            {
                AddNewField();
            }

            // 代码生成按钮
            if (GUILayout.Button("生成数据结构类/配置加载类/配置管理器", GUILayout.Width(300)))
            {
                GenerateCode();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 添加新字段到当前选中的配置
        /// 包含空配置校验、字段名空校验、自动重名处理
        /// </summary>
        private void AddNewField()
        {
            // 未选中配置校验
            if (selectConfigData == null)
            {
                EditorUtility.DisplayDialog("错误", "未选中任何配置，无法添加字段", "确定");
                return;
            }

            // 字段名空校验
            if (string.IsNullOrEmpty(_newFieldName))
            {
                EditorUtility.DisplayDialog("错误", "字段名称不能为空", "确定");
                return;
            }

            // 生成带索引的字段名（避免重名）
            var currentNewFieldName = $"{_newFieldName}{fieldIndex++}";
            var columnTemplate = new FieldTemplate(currentNewFieldName, _newFieldType);
            selectConfigData.columns.Add(columnTemplate);

            // 为所有行数据添加该字段的默认值
            foreach (var row in selectConfigData.rows)
            {
                var value = string.Empty;
                switch (_newFieldType)
                {
                    case E_FieldType.Int:
                        value = 0.ToString();
                        break;
                    case E_FieldType.Float:
                        value = 0.0f.ToString();
                        break;
                    case E_FieldType.Bool:
                        value = false.ToString();
                        break;
                }

                // 自动处理重名字段（循环生成新名称直到不重复）
                while(!row.TryAdd(currentNewFieldName, value))
                {
                    selectConfigData.columns.Remove(columnTemplate);
                    currentNewFieldName = $"{_newFieldName}{fieldIndex++}";
                    columnTemplate = new FieldTemplate(currentNewFieldName, _newFieldType);
                    selectConfigData.columns.Add(columnTemplate);
                    if (row.TryAdd(currentNewFieldName, value))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 绘制数据表格编辑区域
        /// 支持行选中、单元格编辑、行删除
        /// </summary>
        private void DrawDataEditorArea()
        {
            EditorGUILayout.BeginVertical("box");

            GUILayout.Label("数据表格编辑", EditorStyles.boldLabel);
            GUILayout.Space(5);

            _tableScrollPos = EditorGUILayout.BeginScrollView(_tableScrollPos, GUILayout.Height(350));

            if(selectConfigData != null)
            {
                // 绘制表格表头
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("序号", GUILayout.Width(50));
                foreach (var col in selectConfigData.columns)
                {
                    GUILayout.Label(col.fieldName, GUILayout.Width(120));
                }
                GUILayout.Label("操作", GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();

                // 绘制表格行数据
                for (var i = 0; i < selectConfigData.rows.Count; i++)
                {
                    // 选中行高亮
                    if (_selectedRowIndex == i)
                    {
                        GUI.backgroundColor = Color.gray;
                    }

                    EditorGUILayout.BeginHorizontal("box");
                    // 行序号
                    GUILayout.Label((i + 1).ToString(), GUILayout.Width(50));

                    // 单元格编辑（按字段类型绘制对应编辑器）
                    foreach (var col in selectConfigData.columns)
                    {
                        var newValue = DrawField(col.fieldType, selectConfigData.rows[i].GetValue(col.fieldName), GUILayout.Width(120));
                        // 根据字段类型更新行数据
                        switch (col.fieldType)
                        {
                            case E_FieldType.Int:
                                selectConfigData.rows[i].SetValue(col.fieldName, ((int)newValue).ToString());
                                break;
                            case E_FieldType.Float:
                                selectConfigData.rows[i].SetValue(col.fieldName, ((float)newValue).ToString());
                                break;
                            case E_FieldType.String:
                                selectConfigData.rows[i].SetValue(col.fieldName, newValue.ToString());
                                break;
                            case E_FieldType.Bool:
                                selectConfigData.rows[i].SetValue(col.fieldName, ((bool)newValue).ToString());
                                break;
                        }
                    }

                    var isDelete = false;
                    // 行删除按钮
                    if (GUILayout.Button("×", GUILayout.Width(30)))
                    {
                        selectConfigData.rows.RemoveAt(i);
                        if (_selectedRowIndex == i)
                        {
                            _selectedRowIndex = -1;
                        }
                        isDelete = true;
                    }

                    EditorGUILayout.EndHorizontal();

                    // 处理删除后的循环中断
                    if (isDelete)
                    {
                        break;
                    }

                    // 恢复背景色
                    if (_selectedRowIndex == i)
                    {
                        GUI.backgroundColor = Color.white;
                    }

                    // 行选中逻辑（鼠标左键点击）
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        var rowRect = GUILayoutUtility.GetLastRect();
                        if (rowRect.Contains(Event.current.mousePosition))
                        {
                            _selectedRowIndex = i;
                            Repaint();
                        }
                    }
                }
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 根据字段类型绘制对应的编辑器控件
        /// 并处理默认值初始化
        /// </summary>
        /// <param name="type">字段类型</param>
        /// <param name="value">当前字段值</param>
        /// <param name="options">布局参数</param>
        /// <returns>编辑后的新值</returns>
        private object DrawField(E_FieldType type, string value, params GUILayoutOption[] options)
        {
            var tempValue = value;
            // 空值处理（初始化默认值）
            if (string.IsNullOrEmpty(tempValue))
            {
                switch (type)
                {
                    case E_FieldType.None:
                    case E_FieldType.String:
                        break;
                    case E_FieldType.Int:
                        tempValue = 0.ToString();
                        break;
                    case E_FieldType.Float:
                        tempValue = 0.0f.ToString();
                        break;
                    case E_FieldType.Bool:
                        tempValue = false.ToString();
                        break;
                }
            }

            // 根据类型绘制对应编辑器
            return type switch
            {
                E_FieldType.Int => EditorGUILayout.IntField(int.Parse(tempValue), options),
                E_FieldType.Float => EditorGUILayout.FloatField(float.Parse(tempValue), options),
                E_FieldType.Bool => EditorGUILayout.Toggle(bool.Parse(tempValue), options),
                E_FieldType.String => EditorGUILayout.TextField(tempValue, options),
                _ => tempValue,
            };
        }

        /// <summary>
        /// 绘制底部操作按钮（新增行、删除选中行）
        /// </summary>
        private void DrawBottomButtons()
        {
            if (selectConfigData == null)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();

            // 新增行按钮
            if (GUILayout.Button("新增行", GUILayout.Width(100)))
            {
                var isMul = false;
                // 检查重复字段名
                for (var i = 0; i < selectConfigData.columns.Count; i++)
                {
                    for (var j = i + 1; j < selectConfigData.columns.Count; j++)
                    {
                        if (string.Equals(selectConfigData.columns[i].fieldName, selectConfigData.columns[j].fieldName))
                        {
                            isMul = true;
                            EditorUtility.DisplayDialog("错误", $"存在重复字段名{selectConfigData.columns[i].fieldName}", "确定");
                        }
                    }
                }

                // 无重复字段名时新增行
                if (!isMul)
                {
                    selectConfigData.rows.Add(new EntryData(selectConfigData.columns));
                    _selectedRowIndex = selectConfigData.rows.Count - 1;
                }
            }

            // 删除选中行按钮
            if (GUILayout.Button("删除选中行", GUILayout.Width(100)))
            {
                if (_selectedRowIndex >= 0 && _selectedRowIndex < selectConfigData.rows.Count)
                {
                    selectConfigData.rows.RemoveAt(_selectedRowIndex);
                    _selectedRowIndex = -1;
                }
            }

            EditorGUILayout.EndHorizontal();
        }
        #endregion

        /// <summary>
        /// 加载所有配置文件（从本地路径读取）
        /// </summary>
        private void LoadConfigs()
        {
            // 清空现有列表
            configDatas.Clear();
            
            // 路径检查（不存在则创建）
            if (!Directory.Exists(GetSavePath()))
            {
                Directory.CreateDirectory(GetSavePath());
                Debug.Log($"路径{GetSavePath()}不存在，已创建");
                AssetDatabase.Refresh();
            }

            // 获取路径下所有文件
            var fileInfos = new DirectoryInfo(GetSavePath()).GetFiles();
            
            // 遍历反序列化配置文件
            foreach (var fileInfo in fileInfos)
            {
                // 跳过meta文件
                if (fileInfo.Extension == ".meta")
                {
                    continue;
                }

                Debug.Log($"已加载路径:{fileInfo}");
                var json = File.ReadAllText($"{fileInfo}");
                var configData = JsonUtility.FromJson<ConfigData>(json);
                configDatas.Add(configData);
            }
        }

        /// <summary>
        /// 保存当前选中的配置到本地文件
        /// </summary>
        private void SaveConfig()
        {
            if (selectConfigData == null)
            {
                return;
            }

            var savePath = $"{GetSavePath()}{selectConfigData.configName}.json";
            File.WriteAllText(savePath, JsonUtility.ToJson(selectConfigData, true));
            AssetDatabase.Refresh();
            Debug.Log($"保存:{selectConfigData.configName}成功");
        }

        /// <summary>
        /// 保存所有配置到本地文件
        /// </summary>
        private void SaveConfigs()
        {
            foreach (var configData in configDatas)
            {
                var savePath = $"{GetSavePath()}{configData.configName}.json";
                var json = JsonUtility.ToJson(configData, true);
                File.WriteAllText(savePath, json);
                AssetDatabase.Refresh();
                Debug.Log($"保存:{configData.configName}成功");
            }
        }

        /// <summary>
        /// 生成配置对应的代码文件
        /// </summary>
        private void GenerateCode()
        {
            // 未选中配置校验
            if (selectConfigData == null)
            {
                EditorUtility.DisplayDialog("提示", "请先选中配置数据", "确定");
                return;
            }
        
            // 无字段校验
            if (selectConfigData.columns.Count == 0)
            {
                EditorUtility.DisplayDialog("提示", "配置无字段模板！", "确定");
                return;
            }

            // 初始化代码生成器并执行生成
            scriptGenerator ??= new ConfigDataGenerator(selectConfigData);
            scriptGenerator.GenerateScript();
        }

        /// <summary>
        /// 获取配置文件保存的完整路径（Assets目录+自定义路径）
        /// </summary>
        /// <returns>完整保存路径</returns>
        private string GetSavePath()
        {
            return $"{Application.dataPath}/{configSavePath}";
        }
    }
}