using System.IO;
using Core.Global;
using Editor.Generation;
using Editor.Generation.Detail;
using UnityEditor;
using UnityEngine;

namespace Editor.Menu
{
    /// <summary>
    /// 菜单栏工具
    /// </summary>
    public static class MenuItemTools
    {
        /// <summary>
        /// SO全局配置保存路径
        /// </summary>
        public const string AssetPath = "Assets/Resources/Global/";

        /// <summary>
        /// 创建全局配置
        /// </summary>
        [MenuItem("GameTool/SO Config/Create GlobalSettings")]
        public static void CreateGlobalSetting()
        {
            // 创建文件夹
            if (!Directory.Exists(AssetPath))
            {
                Directory.CreateDirectory(AssetPath);
            }

            // 创建ScriptableObject对象
            var settings = ScriptableObject.CreateInstance<GlobalSettings>();
            // 创建编辑器资源
            AssetDatabase.CreateAsset(settings, AssetPath + "GlobalSettings.asset");
            // 保存资源
            AssetDatabase.SaveAssets();
            // 刷新
            AssetDatabase.Refresh();

            Debug.Log($"已创建GlobalSetting配置{AssetPath}{nameof(GlobalSettings)}");
        }

        /// <summary>
        /// 生成InputActionData
        /// </summary>
        [MenuItem("GameTool/Generate/Generate InputActionData")]
        public static void GenerateInputActionDataScript()
        {
            IScriptGenerator scriptGenerator = new InputActionDataGenerator();
            scriptGenerator.GenerateScript();
        }

        /// <summary>
        /// 合并网格
        /// </summary>
        [MenuItem("GameTool/Mesh/Combine Meshes")]
        public static void CombineMesh()
        {
            // 获取选中的对象
            var gameObject = Selection.activeGameObject;
            if (gameObject == null)
            {
                Debug.LogError($"选中的对象为null");
                return;
            }

            var meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            if (meshFilters.Length == 0)
            {
                Debug.LogError($"对象上不存在MeshFilter");
                return;
            }

            var combineInstances = new CombineInstance[meshFilters.Length];

            for (var i = 0; i < combineInstances.Length; i++)
            {
                // 初始化
                combineInstances[i].mesh = meshFilters[i].sharedMesh;
                // 变换网格
                combineInstances[i].transform = gameObject.transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;
                // 销毁原对象，可选
                // GameObject.Destroy(meshFilters[i].gameObject);
            }

            // 创建新网格
            var mesh = new Mesh();

            // 总顶点数
            var totalVertices = 0;
            foreach (var item in combineInstances)
            {
                totalVertices += item.mesh.vertexCount;
            }

            if (totalVertices > ushort.MaxValue)
            {
                // 超过最大限制为65535，即ushort，索引格式用UInt32
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            }
            else
            {
                // 未超过最大索引数，默认就是UInt16
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
            }

            // 合并网格
            mesh.CombineMeshes(combineInstances, true, true, true);
            // 重新计算包围盒
            mesh.RecalculateBounds();

            // 创建资源
            AssetDatabase.CreateAsset(mesh, "Assets/Editor/ArtRes/Mesh/MergedMesh.asset");

            #region ����ʱ�߼�
            // 可添加网格过滤器
            //MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            // 设置网格
            //meshFilter.mesh = mesh;
            // 可添加网格渲染器
            //MeshRenderer meshRenderer = gameObject.gameObject.AddComponent<MeshRenderer>();
            //meshRenderer.sharedMaterial = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;
            #endregion
        }

    }
}
