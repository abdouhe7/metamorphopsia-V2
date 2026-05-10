using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRatio : MonoBehaviour
{
   
    [Header("Grid Cells")]
    public int cellsX = 18;
    public int cellsY = 10;
[Header("Vertex Points")]
public GameObject vertexPointPrefab;
public float pointSize = 0.08f;

private GameObject[] vertexPoints;
    [Header("World Size")]
    public float meshWidth = 18f;
    public float meshHeight = 10f;

    [Header("Subdivision")]
    public int subdivisionLevel = 1;

    private Mesh mesh;

    void Start()
    {
        CreateGrid();
    }

    void OnValidate()
    {
        if (cellsX < 1) cellsX = 1;
        if (cellsY < 1) cellsY = 1;

        if (meshWidth <= 0f) meshWidth = 1f;
        if (meshHeight <= 0f) meshHeight = 1f;

        if (subdivisionLevel < 1)
            subdivisionLevel = 1;
    }

    [ContextMenu("Create Grid")]
    public void CreateGrid()
    {
        MeshFilter mf = GetComponent<MeshFilter>();

        if (mf == null)
            return;

        mesh = new Mesh();

        mesh.name =
            $"Grid_{cellsX}x{cellsY}_Level{subdivisionLevel}";

        int factor =
            (int)Mathf.Pow(2, subdivisionLevel - 1);

        int finalCellsX = cellsX * factor;
        int finalCellsY = cellsY * factor;

        int cols = finalCellsX + 1;
        int rows = finalCellsY + 1;

        Vector3[] vertices =
            new Vector3[cols * rows];

        Vector2[] uv =
            new Vector2[cols * rows];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int index = x + y * cols;

                float u = (float)x / finalCellsX;
                float v = (float)y / finalCellsY;

                float posX =
                    (u - 0.5f) * meshWidth;

                float posY =
                    (v - 0.5f) * meshHeight;

                vertices[index] =
                    new Vector3(posX, posY, 0f);

                uv[index] =
                    new Vector2(u, v);
            }
        }

        int[] triangles =
            new int[finalCellsX * finalCellsY * 6];

        int t = 0;

        for (int y = 0; y < finalCellsY; y++)
        {
            for (int x = 0; x < finalCellsX; x++)
            {
                int bl = x + y * cols;
                int br = (x + 1) + y * cols;
                int tl = x + (y + 1) * cols;
                int tr = (x + 1) + (y + 1) * cols;

                triangles[t++] = bl;
                triangles[t++] = tl;
                triangles[t++] = tr;

                triangles[t++] = bl;
                triangles[t++] = tr;
                triangles[t++] = br;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        mesh.RecalculateNormals();

        mf.sharedMesh = mesh;
        CreateVertexPoints(vertices);

        Debug.Log(
            $"Cells = {finalCellsX} x {finalCellsY}, " +
            $"Vertices = {mesh.vertexCount}, " +
            $"Triangles = {mesh.triangles.Length / 3}"
        );
    }
    void CreateVertexPoints(Vector3[] vertices)
{
    if (vertexPointPrefab == null)
    {
        Debug.LogWarning("Vertex Point Prefab is not assigned.");
        return;
    }

    if (vertexPoints != null)
    {
        for (int i = 0; i < vertexPoints.Length; i++)
        {
            if (vertexPoints[i] != null)
                DestroyImmediate(vertexPoints[i]);
        }
    }

    vertexPoints = new GameObject[vertices.Length];

    for (int i = 0; i < vertices.Length; i++)
    {
        GameObject point = Instantiate(vertexPointPrefab, transform);

        // نخلي النقطة أمام الشبكة قليلًا باتجاه الكاميرا
        point.transform.localPosition = vertices[i] + new Vector3(0f, 0f, -0.08f);
        point.transform.localRotation = Quaternion.identity;
        point.transform.localScale = Vector3.one * pointSize;

        VertexPoint vp = point.GetComponent<VertexPoint>();
        if (vp != null)
        {
            vp.vertexIndex = i;
            vp.weight = 1f;
        }

        SphereCollider col = point.GetComponent<SphereCollider>();
        if (col != null)
        {
            col.radius = 1.5f; // يكبر منطقة الالتقاط
        }

        vertexPoints[i] = point;
    }
}

    void OnDrawGizmos()
    {
        MeshFilter mf = GetComponent<MeshFilter>();

        if (mf == null || mf.sharedMesh == null)
            return;

        Vector3[] vertices =
            mf.sharedMesh.vertices;

        int factor =
            (int)Mathf.Pow(2, subdivisionLevel - 1);

        int finalCellsX = cellsX * factor;
        int finalCellsY = cellsY * factor;

        int cols = finalCellsX + 1;
        int rows = finalCellsY + 1;

        if (vertices.Length != cols * rows)
            return;

        Gizmos.color = Color.black;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int index = x + y * cols;

                Vector3 p =
                    transform.TransformPoint(vertices[index]);

                if (x < cols - 1)
                {
                    int right =
                        (x + 1) + y * cols;

                    Gizmos.DrawLine(
                        p,
                        transform.TransformPoint(vertices[right])
                    );
                }

                if (y < rows - 1)
                {
                    int up =
                        x + (y + 1) * cols;

                    Gizmos.DrawLine(
                        p,
                        transform.TransformPoint(vertices[up])
                    );
                }
            }
        }
    }
}