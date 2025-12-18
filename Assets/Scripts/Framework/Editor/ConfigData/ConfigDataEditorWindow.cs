using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 配置数据编辑器
/// </summary>
public class ConfigDataEditorWindow : EditorWindow
{
    // 字段索引
    private static uint fieldIndex = 1;

    // 选择的配置数据
    private ConfigData selectConfigData;
    private string configName = "NewInfo";

    private List<RowData> _rowDatas = new List<RowData>();
    private int _selectedRowIndex = -1;

    // 滚动位置
    private Vector2 _columnScrollPos;
    private Vector2 _tableScrollPos;

    // 临时编辑数据
    private string _newFieldName = $"newField";
    private E_FieldType _newFieldType = E_FieldType.None;

    // 左侧配置数据列表
    private readonly List<ConfigData> configDatas = new List<ConfigData>();

    private string searchText;
    private string jsonOutPath;
    private string fileName;
    private bool prettyPrint;
    private Vector2 scrollPosition;
    private readonly string configSavePath = $"{Application.dataPath}/ConfigData/";

    private static ConfigDataEditorWindow configDataEditorWindow;

    // 窗口入口
    [MenuItem("GameTool/EditorWindow/Config Data Editor")]
    public static void OpenWindow()
    {
        configDataEditorWindow = GetWindow<ConfigDataEditorWindow>("Config Data Editor");
        configDataEditorWindow.minSize = new Vector2(900, 700);
        fieldIndex = 1;
        configDataEditorWindow.LoadConfig(); // 加载已有配置
    }

    #region UI绘制
    private void OnGUI()
    {
        ToolbarArea();
        EditorGUILayout.BeginHorizontal();

        DrawConfigDatasArea();
        DrawFieldTempleteArea();

        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 工具栏区域
    /// </summary>
    private void ToolbarArea()
    {
        //工具栏水平布局
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        // 搜索框
        searchText = EditorGUILayout.TextField(searchText, EditorStyles.toolbarSearchField, GUILayout.Width(300));
        //json保存路径输入框
        GUILayout.Label(new GUIContent("JsonSavePath", "Json文件保存路径"), EditorStyles.boldLabel, GUILayout.Width(90));
        jsonOutPath = EditorGUILayout.TextField(jsonOutPath, EditorStyles.toolbarTextField);
        //选项框，是否格式化输出
        prettyPrint = GUILayout.Toggle(prettyPrint, "格式化Json");
        // 序列化按钮：点击可以将对象序列化为json文件
        if (GUILayout.Button("生成Json"))
        {
            //SaveJson(Encoding.UTF8);
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 绘制配置数据区域
    /// </summary>
    private void DrawConfigDatasArea()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.Width(300));
        GUILayout.Label("配置数据区域", EditorStyles.boldLabel);
        // 绘制配置数据
        DrawConfigDatas();
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 绘制配置数据
    /// </summary>
    private void DrawConfigDatas()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        bool isDel = false;
        foreach (ConfigData configData in configDatas)
        {
            if (string.IsNullOrEmpty(searchText) || configData.configName.Contains(searchText))
            {
                EditorGUILayout.BeginHorizontal();

                // 选择按钮
                if (GUILayout.Button(configData.configName, EditorStyles.miniButtonLeft))
                {
                    selectConfigData = configData;
                }

                // 删除按钮
                if (GUILayout.Button("Delete", EditorStyles.miniButtonRight, GUILayout.Width(60)))
                {
                    if (EditorUtility.DisplayDialog("提示", $"确定要删除配置数据 '{configData}' 吗？", "是", "否"))
                    {
                        //DeleteBuff(buff);
                        isDel = true;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            if (isDel)
            {
                break;
            }
        }

        EditorGUILayout.EndScrollView();

        // 新建按钮
        EditorGUILayout.Space();

        // 类名编辑
        configName = EditorGUILayout.TextField("类名", configName);
        GUILayout.Space(10);

        if (GUILayout.Button("创建配置"))
        {
            CreateConfigData();
        }
    }

    /// <summary>
    /// 创建配置数据
    /// </summary>
    private void CreateConfigData()
    {
        if (string.IsNullOrEmpty(configName))
        {
            EditorUtility.DisplayDialog("提示", "类名称不能为空", "确定");
            return;
        }

        // 创建新的配置数据
        if (!Directory.Exists(configSavePath))
        {
            Directory.CreateDirectory(configSavePath);
        }

        string assetPath = $"{configSavePath}{configName}.bytes";
        if (File.Exists(assetPath))
        {
            EditorUtility.DisplayDialog("提示", $"已存在名为 '{configName}' 的配置数据", "确定");
            return;
        }

        ConfigData newConfigData = new ConfigData(configName);
        using FileStream fs = new FileStream(assetPath, FileMode.Create, FileAccess.Write);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, newConfigData);
        fs.Close();
        AssetDatabase.Refresh();
        Debug.Log($"创建路径:{assetPath}");

        //刷新配置列表
        RefresConfigArea();
    }

    /// <summary>
    /// 刷新配置数据区域
    /// </summary>
    private void RefresConfigArea()
    {
        //清空缓存
        configDatas.Clear();

        FileInfo[] fileInfos = Directory.CreateDirectory(configSavePath).GetFiles();
        foreach (FileInfo fileInfo in fileInfos)
        {
            if (fileInfo.Extension == ".meta")
            {
                continue;
            }

            Debug.Log($"读取路径:{configSavePath}{fileInfo.Name}");
            using FileStream fs = File.Open($"{configSavePath}{fileInfo.Name}", FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            ConfigData configData = bf.Deserialize(fs) as ConfigData;
            fs.Close();
            configDatas.Add(configData);
        }
    }

    /// <summary>
    /// 绘制字段模板区域
    /// </summary>
    private void DrawFieldTempleteArea()
    {
        EditorGUILayout.BeginVertical();

        // 字段模板编辑区域
        DrawColumnTemplateArea();
        GUILayout.Space(10);

        // 表格编辑区域
        DrawTableArea();
        GUILayout.Space(10);

        // 底部按钮
        DrawBottomButtons();

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 绘制列模板编辑区域
    /// </summary>
    private void DrawColumnTemplateArea()
    {
        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("列模板配置", EditorStyles.boldLabel);

        _columnScrollPos = EditorGUILayout.BeginScrollView(_columnScrollPos, GUILayout.Height(150));

        // 列模板表头
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("主键", GUILayout.Width(50));
        GUILayout.Label("字段名", GUILayout.Width(250));
        GUILayout.Label("字段类型", GUILayout.Width(100));
        GUILayout.Label("字段描述", GUILayout.ExpandWidth(true));
        GUILayout.Label("操作", GUILayout.Width(50));

        EditorGUILayout.EndHorizontal();

        if (selectConfigData != null)
        {
            bool isDelete = false;
            // 列模板列表
            for (int i = 0; i < selectConfigData.columns.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                selectConfigData.columns[i].key = EditorGUILayout.Toggle(selectConfigData.columns[i].key, GUILayout.Width(50));

                // 当字段名称更新后，要同步到所有的行数据
                string oldFieldName = selectConfigData.columns[i].fieldName;
                selectConfigData.columns[i].fieldName = EditorGUILayout.TextField(selectConfigData.columns[i].fieldName, GUILayout.Width(250));
                if (selectConfigData.columns[i].fieldName != oldFieldName)
                {
                    foreach (RowData rowData in _rowDatas)
                    {
                        string oldValue = rowData.fieldToValueMap[oldFieldName];
                        rowData.fieldToValueMap.Remove(oldFieldName);
                        if (!rowData.TryAdd(selectConfigData.columns[i].fieldName, oldValue))
                        {
                            EditorUtility.DisplayDialog("错误", $"该字段名（{selectConfigData.columns[i].fieldName}）已存在", "确定");
                            selectConfigData.columns[i].fieldName = oldFieldName;
                            rowData.fieldToValueMap.Add(oldFieldName, oldValue);
                        }
                    }
                }

                // 当字段类型更新后，要同步到所有的行数据
                E_FieldType oldType = selectConfigData.columns[i].fieldType;
                selectConfigData.columns[i].fieldType = (E_FieldType)EditorGUILayout.EnumPopup(selectConfigData.columns[i].fieldType, GUILayout.Width(100));
                if (selectConfigData.columns[i].fieldType != oldType)
                {
                    foreach (RowData rowData in _rowDatas)
                    {
                        string value = string.Empty;
                        switch (selectConfigData.columns[i].fieldType)
                        {
                            case E_FieldType.Int:
                                value = default(int).ToString();
                                break;
                            case E_FieldType.Float:
                                value = default(float).ToString();
                                break;
                            case E_FieldType.Bool:
                                value = default(bool).ToString();
                                break;
                        }
                        rowData.fieldToValueMap[selectConfigData.columns[i].fieldName] = value;
                    }
                }

                selectConfigData.columns[i].fieldDescription = EditorGUILayout.TextField(selectConfigData.columns[i].fieldDescription, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("×", GUILayout.Width(50)))
                {
                    // 刷新行数据的字段
                    foreach (var row in _rowDatas)
                    {
                        row.fieldToValueMap.Remove(selectConfigData.columns[i].fieldName);
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

        // 模板操作按钮
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("添加新字段", GUILayout.Width(200)))
        {
            AddNewField();
        }

        if (GUILayout.Button("生成二进制数据", GUILayout.Width(200)))
        {
            SaveConfig();
        }
        if (GUILayout.Button("生成数据结构类", GUILayout.Width(200)))
        {
            GenerateCode();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 添加新字段
    /// </summary>
    private void AddNewField()
    {
        if (selectConfigData == null)
        {
            EditorUtility.DisplayDialog("错误", "未选择任何配置，无法添加！", "确定");
            return;
        }

        if (string.IsNullOrEmpty(_newFieldName))
        {
            EditorUtility.DisplayDialog("错误", "字段名不能为空！", "确定");
            return;
        }

        string currentNewFieldName = $"{_newFieldName}{fieldIndex++}";
        ColumnTemplate columnTemplate = new ColumnTemplate(currentNewFieldName, _newFieldType);
        selectConfigData.columns.Add(columnTemplate);

        // 为现有行添加新列的默认值
        foreach (var row in _rowDatas)
        {
            string value = string.Empty;
            switch (_newFieldType)
            {
                case E_FieldType.Int:
                    value = default(int).ToString();
                    break;
                case E_FieldType.Float:
                    value = default(float).ToString();
                    break;
                case E_FieldType.Bool:
                    value = default(bool).ToString();
                    break;
            }

            while(!row.TryAdd($"{currentNewFieldName}", value))
            {
                selectConfigData.columns.Remove(columnTemplate);
                currentNewFieldName = $"{_newFieldName}{fieldIndex++}";
                columnTemplate = new ColumnTemplate(currentNewFieldName, _newFieldType);
                selectConfigData.columns.Add(columnTemplate);
                if (row.TryAdd($"{currentNewFieldName}", value))
                {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 绘制表格编辑区域
    /// </summary>
    private void DrawTableArea()
    {
        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("数据表格", EditorStyles.boldLabel);
        GUILayout.Space(5);

        _tableScrollPos = EditorGUILayout.BeginScrollView(_tableScrollPos, GUILayout.Height(350));

        if(selectConfigData != null)
        {
            // 表格表头
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("行号", GUILayout.Width(50));
            foreach (var col in selectConfigData.columns)
            {
                GUILayout.Label(col.fieldName, GUILayout.Width(120));
            }

            GUILayout.Label("操作", GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            // 表格内容
            for (int i = 0; i < _rowDatas.Count; i++)
            {
                // 选中行高亮
                if (_selectedRowIndex == i)
                {
                    GUI.backgroundColor = Color.gray;
                }

                EditorGUILayout.BeginHorizontal("box");
                // 行号
                GUILayout.Label((i + 1).ToString());

                // 单元格编辑
                foreach (var col in selectConfigData.columns)
                {
                    object newValue = DrawField(col.fieldType, _rowDatas[i].GetValue(col.fieldName));
                    // GUI渲染位置不对
                    switch (col.fieldType)
                    {
                        case E_FieldType.Int:
                            _rowDatas[i].SetValue(col.fieldName, ((int)newValue).ToString());
                            break;
                        case E_FieldType.Float:
                            _rowDatas[i].SetValue(col.fieldName, ((float)newValue).ToString());
                            break;
                        case E_FieldType.String:
                            _rowDatas[i].SetValue(col.fieldName, newValue.ToString());
                            break;
                        case E_FieldType.Bool:
                            _rowDatas[i].SetValue(col.fieldName, ((bool)newValue).ToString());
                            break;
                    }
                }

                bool isDelete = false;
                // 删除行按钮
                if (GUILayout.Button("×", GUILayout.Width(30)))
                {
                    _rowDatas.RemoveAt(i);
                    if (_selectedRowIndex == i)
                    {
                        _selectedRowIndex = -1;
                    }
                    isDelete = true;
                }

                EditorGUILayout.EndHorizontal();

                // GIUI匹配完整匹配后，再退出循环
                if (isDelete)
                {
                    break;
                }

                // 恢复背景色
                if (_selectedRowIndex == i)
                {
                    GUI.backgroundColor = Color.white;
                }

                // 行选中逻辑
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    Rect rowRect = GUILayoutUtility.GetLastRect();
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
    /// 绘制不同类型的字段编辑框
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    private object DrawField(E_FieldType type, string value, params GUILayoutOption[] options)
    {
        if (value == null)
        {
            return value?.ToString();
        }
        
        return type switch
        {
            E_FieldType.Int => EditorGUILayout.IntField(int.Parse(value), options),
            E_FieldType.Float => EditorGUILayout.FloatField(float.Parse(value), options),
            E_FieldType.Bool => EditorGUILayout.Toggle(bool.Parse(value), options),
            E_FieldType.String => EditorGUILayout.TextField(value, options),
            _ => value,
        };
    }

    /// <summary>
    /// 绘制底部按钮
    /// </summary>
    private void DrawBottomButtons()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加行", GUILayout.Width(100)))
        {
            // 检查若有重复字段名则提示
            foreach (var item in selectConfigData.columns)
            {

            }

            _rowDatas.Add(new RowData(selectConfigData.columns));
            _selectedRowIndex = _rowDatas.Count - 1;
        }
        if (GUILayout.Button("删除选中行", GUILayout.Width(100)))
        {
            if (_selectedRowIndex >= 0 && _selectedRowIndex < _rowDatas.Count)
            {
                _rowDatas.RemoveAt(_selectedRowIndex);
                _selectedRowIndex = -1;
            }
        }
        if (GUILayout.Button("保存数据", GUILayout.Width(100)))
        {
            SaveConfig();
        }
        EditorGUILayout.EndHorizontal();
    }
    #endregion

    #region 数据加载/保存
    /// <summary>
    /// 加载配置（列模板+行数据）
    /// </summary>
    private void LoadConfig()
    {
        if (!Directory.Exists(configSavePath))
        {
            return;
        }

        // 获取所有配置数据文件
        FileInfo[] fileInfos = Directory.CreateDirectory(configSavePath).GetFiles();

        BinaryFormatter bf = new BinaryFormatter();
        // 遍历文件信息
        foreach (FileInfo fileInfo in fileInfos)
        {
            if (fileInfo.Extension == ".meta")
            {
                continue;
            }

            Debug.Log($"加载路径：{configSavePath}{fileInfo.Name}");
            using FileStream fs = File.OpenRead($"{configSavePath}{fileInfo.Name}");
            ConfigData configData = bf.Deserialize(fs) as ConfigData;
            fs.Close();
            configDatas.Add(configData);
        }
    }

    /// <summary>
    /// 保存配置
    /// </summary>
    private void SaveConfig()
    {
        if (selectConfigData == null)
        {
            return;
        }
        string savePath = $"{configSavePath}{selectConfigData.configName}";
        using FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, selectConfigData);
        fs.Close();
        AssetDatabase.Refresh();
        Debug.Log($"配置保存成功：{savePath}");
    }
    #endregion

    #region 代码生成
    private void GenerateCode()
    {
        if (selectConfigData.columns.Count == 0)
        {
            EditorUtility.DisplayDialog("提示", "请先添加列模板！", "确定");
            return;
        }

        // 构建代码内容
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine();
        sb.AppendLine("[Serializable]");
        sb.AppendLine($"public class {selectConfigData.configName}Info");
        sb.AppendLine("{");
        foreach (var col in selectConfigData.columns)
        {
            string typeStr = GetCSharpType(col.fieldType);
            sb.AppendLine($"    public {typeStr} {col.fieldName}; // {col.fieldType}类型");
        }
        sb.AppendLine("}");

        // 保存代码文件
        string path = Application.dataPath + $"/Scripts/ConfigInfo/{selectConfigData.configName}Info.cs";
        File.WriteAllText(path, sb.ToString()); 
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("成功", "代码生成成功！", "确定");
    }

    // 转换为C#类型字符串
    private string GetCSharpType(E_FieldType type)
    {
        return type switch
        {
            E_FieldType.Int => "int",
            E_FieldType.Float => "float",
            E_FieldType.Bool => "bool",
            E_FieldType.String => "string",
            _ => "object",
        };
    }
    #endregion


}
