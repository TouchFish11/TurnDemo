using UnityEngine;

public class Lesson46 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CombineMesh();
    }

    private void CombineMesh()
    {
        MeshFilter[] meshFilters = this.GetComponentsInChildren<MeshFilter>();

        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < combineInstances.Length; i++)
        {
            // ��ȡ��������
            combineInstances[i].mesh = meshFilters[i].sharedMesh;
            // ���ڽ��Ӷ���Ķ���λ�ôӵ�ǰ���ؿռ�任��������ı��ؿռ�
            combineInstances[i].transform = this.transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;
            // ���д����������١�ʧ�
            Destroy(meshFilters[i].gameObject);

        }

        // ����������
        Mesh mesh = new Mesh();

        // �ж϶������Ƿ񳬹�������
        int totalVertices = 0;
        foreach (var item in combineInstances)
        {
            totalVertices += item.mesh.vertexCount;
        }

        if (totalVertices > ushort.MaxValue)
        {
            // Ĭ����UInt16��ushort�����������֧��65535�����㡣���ϲ��Ķ�����������ֵ����Ҫ�޸�ΪUint32��uint��
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }
        else
        {
            // Ĭ����UInt16��ushort�����������֧��65535�����㡣���ϲ��Ķ�����������ֵ����Ҫ�޸�ΪUint32��uint��
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
        }

        // �ϲ�����
        mesh.CombineMeshes(combineInstances, true, true, true);

        // ���¼����Χ��
        mesh.RecalculateBounds();

        // ʹ����Ⱦ�ϲ�������
        MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        // ��̬�����Ⱦ�������ò�����
        MeshRenderer meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;
    }
}
