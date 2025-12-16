using CustomEditor.ScriptGeneration;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 菜单栏工具类
/// </summary>
public class MenuItemTools
{
    /// <summary>
    /// SO资源存储路径
    /// </summary>
    public const string AssetPath = "Assets/Resources/Global/";

    /// <summary>
    /// 创建全局配置文件
    /// </summary>
    [MenuItem("GameTool/SO Config/Create GlobalSettings")]
    public static void CreateGlobalSetting()
    {
        //不存在则创建
        if (!Directory.Exists(AssetPath))
        {
            Directory.CreateDirectory(AssetPath);
        }

        //创建ScriptableObject实例
        GlobalSettings settings = ScriptableObject.CreateInstance<GlobalSettings>();
        //创建配置文件
        AssetDatabase.CreateAsset(settings, AssetPath + "GlobalSettings.asset");
        //保存文件
        AssetDatabase.SaveAssets();
        //刷新
        AssetDatabase.Refresh();

        Debug.Log($"已创建GlobalSetting，路径：{AssetPath}{nameof(GlobalSettings)}");
    }

    /// <summary>
    /// 生成ResKeyCollection脚本
    /// </summary>
    [MenuItem("GameTool/Generate/Generate ResKeyCollection")]
    public static void GenerateResKeyCollectionScript()
    {
        ResKeyCollectionClassGenerator resKeyCollectionClassGenerator = new ResKeyCollectionClassGenerator();
        resKeyCollectionClassGenerator.GenerateScript();
    }

    /// <summary>
    /// 合并网格
    /// </summary>
    [MenuItem("GameTool/Mesh/Combine Meshes")]
    public static void CombineMesh()
    {
        // 获取当前选择的对象
        GameObject gameObject = Selection.activeGameObject;
        if (gameObject == null)
        {
            Debug.LogError($"未选择任何对象来合并网格");
            return;
        }

        MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length == 0)
        {
            Debug.LogError($"选择对象的子对象没有网格数据");
            return;
        }

        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < combineInstances.Length; i++)
        {
            // 获取网格数据
            combineInstances[i].mesh = meshFilters[i].sharedMesh;
            // 用于将子对象的顶点位置从当前本地空间变换到父对象的本地空间
            combineInstances[i].transform = gameObject.transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;
            // 自行处理网格（销毁、失活）
            // GameObject.Destroy(meshFilters[i].gameObject);
        }

        // 创建新网格
        Mesh mesh = new Mesh();

        // 判断顶点数是否超过了限制
        int totalVertices = 0;
        foreach (var item in combineInstances)
        {
            totalVertices += item.mesh.vertexCount;
        }

        if (totalVertices > ushort.MaxValue)
        {
            // 默认是UInt16（ushort），代表最多支持65535个顶点。若合并的顶点数超过该值，就要修改为Uint32（uint）
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }
        else
        {
            // 默认是UInt16（ushort），代表最多支持65535个顶点。若合并的顶点数超过该值，就要修改为Uint32（uint）
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
        }

        // 合并网格
        mesh.CombineMeshes(combineInstances, true, true, true);

        // 重新计算包围盒
        mesh.RecalculateBounds();

        // 存储为资源
        AssetDatabase.CreateAsset(mesh, "Assets/Editor/ArtRes/Mesh/MergedMesh.asset");

        #region 运行时逻辑
        //// 使用渲染合并的网格
        //MeshFilter meshFilter = gameObject.gameObject.AddComponent<MeshFilter>();
        //meshFilter.mesh = mesh;
        //// 动态添加渲染器，设置材质球
        //MeshRenderer meshRenderer = gameObject.gameObject.AddComponent<MeshRenderer>();
        //meshRenderer.sharedMaterial = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;
        #endregion
    }

}
