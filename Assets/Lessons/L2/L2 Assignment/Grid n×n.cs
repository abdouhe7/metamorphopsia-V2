using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid4x4Generator : MonoBehaviour
{
    private Mesh mesh;
    private MeshFilter meshFilter;

    [Header("Grid Size")]
    public int cellsX = 4;
    public int cellsY = 4;
    [Header("Subdivision")]
public int subdivision = 1;

    [Header("World Size")]
    public float meshWidth = 4f;
    public float meshHeight = 4f;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        CreateGrid();
    }

    void OnValidate()
    {
        if (cellsX < 1) cellsX = 1;
        if (cellsY < 1) cellsY = 1;
        if (meshWidth <= 0f) meshWidth = 1f;
        if (meshHeight <= 0f) meshHeight = 1f;
        if (subdivision < 1) subdivision = 1;

        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            CreateGrid();
        }
    }

    [ContextMenu("Create Grid")]
    void CreateGrid()
    {
        mesh = new Mesh();
        mesh.name = $"Grid_{cellsX}x{cellsY}";
int finalCellsX = cellsX * subdivision;
int finalCellsY = cellsY * subdivision;

int cols = finalCellsX + 1;
int rows = finalCellsY + 1;
        int totalVertices = cols * rows;

        Vector3[] vertices = new Vector3[totalVertices];
        Vector2[] uv = new Vector2[totalVertices];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int index = x + y * cols;

               float u = (float)x / finalCellsX;
float v = (float)y / finalCellsY;

                float posX = (u - 0.5f) * meshWidth;
                float posY = (v - 0.5f) * meshHeight;

                vertices[index] = new Vector3(posX, posY, 0f);
                uv[index] = new Vector2(u, v);
            }
        }

      int[] triangles = new int[finalCellsX * finalCellsY * 6];
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

        meshFilter.sharedMesh = mesh;

        Debug.Log("Grid Created: " +
                  "Cells = " + (cellsX * cellsY) +
                  ", Vertices = " + mesh.vertexCount +
                  ", Triangles = " + (mesh.triangles.Length / 3));
    }

    void OnDrawGizmos()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null) return;

        Mesh currentMesh = mf.sharedMesh;
        Vector3[] vertices = currentMesh.vertices;

        int finalCellsX = cellsX * subdivision;
int finalCellsY = cellsY * subdivision;

int cols = finalCellsX + 1;
int rows = finalCellsY + 1;

        if (vertices.Length != cols * rows) return;

        Gizmos.color = Color.black;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int index = x + y * cols;
                Vector3 p = transform.TransformPoint(vertices[index]);

                if (x < cols - 1)
                {
                    int right = (x + 1) + y * cols;
                    Vector3 pRight = transform.TransformPoint(vertices[right]);
                    Gizmos.DrawLine(p, pRight);
                }

                if (y < rows - 1)
                {
                    int up = x + (y + 1) * cols;
                    Vector3 pUp = transform.TransformPoint(vertices[up]);
                    Gizmos.DrawLine(p, pUp);
                }
            }
        }
    }
}