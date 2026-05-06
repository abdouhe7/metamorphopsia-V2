using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class StaticQuad : MonoBehaviour
{
    private Mesh mesh;

    void Start()
    {
        CreateQuad();
    }

    void CreateQuad()
    {
        mesh = new Mesh();
        mesh.name = "Static Quad";

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-1f, -1f, 0f), // 0
            new Vector3( 1f, -1f, 0f), // 1
            new Vector3( 1f,  1f, 0f), // 2
            new Vector3(-1f,  1f, 0f)  // 3
        };

        int[] triangles = new int[]
        {
            0, 3, 2,
            0, 2, 1
        };

        Vector2[] uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    void OnDrawGizmosSelected()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null) return;

        Vector3[] v = mf.sharedMesh.vertices;
        if (v.Length < 4) return;

        Vector3 p0 = transform.TransformPoint(v[0]);
        Vector3 p1 = transform.TransformPoint(v[1]);
        Vector3 p2 = transform.TransformPoint(v[2]);
        Vector3 p3 = transform.TransformPoint(v[3]);

        Gizmos.color = Color.blue;

        // حدود المربع
        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p0);

        // الخط الفاصل بين المثلثين
        Gizmos.DrawLine(p0, p2);
    }
}