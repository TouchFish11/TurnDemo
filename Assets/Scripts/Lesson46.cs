using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson46 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CombineMesh();
    }

    // OnInitDataCompleted is called once per frame
    void Update()
    {
        
    }

    private void CombineMesh()
    {
        MeshFilter[] meshFilters = this.GetComponentsInChildren<MeshFilter>();

        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < combineInstances.Length; i++)
        {
            // 获取网格数据
            combineInstances[i].mesh = meshFilters[i].sharedMesh;
            // 用于将子对象的顶点位置从当前本地空间变换到父对象的本地空间
            combineInstances[i].transform = this.transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;
            // 自行处理网格（销毁、失活）
            Destroy(meshFilters[i].gameObject);

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

        // 使用渲染合并的网格
        MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        // 动态添加渲染器，设置材质球
        MeshRenderer meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;
    }
}
