using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ScreenRatioGrid : MonoBehaviour
{
    [Header("Grid Cells")]
    public int cellsX = 18;
    public int cellsY = 10;

    [Header("World Size")]
    public float meshWidth = 18f;
    public float meshHeight = 10f;

    [Header("Subdivision")]
    public int subdivisionLevel = 1;

    private Mesh mesh;

    void Start()
    {
        [ContextMenu("Create Grid")]
public void CreateGrid()
        CreateGrid();
    }

    void OnValidate()
    {
        if (cellsX < 1) cellsX = 1;
        if (cellsY < 1) cellsY = 1;
        if (meshWidth <= 0f) meshWidth = 1f;
        if (meshHeight <= 0f) meshHeight = 1f;
        if (subdivisionLevel < 1) subdivisionLevel = 1;

        if (Application.isPlaying)
        {
            CreateGrid();
        }
    }

    [ContextMenu("Create Grid")]
    public void CreateGrid()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null) return;

        mesh = new Mesh();
        mesh.name = $"Grid_{cellsX}x{cellsY}_Level{subdivisionLevel}";

        int factor = (int)Mathf.Pow(2, subdivisionLevel - 1);

        int finalCellsX = cellsX * factor;
        int finalCellsY = cellsY * factor;

        int cols = finalCellsX + 1;
        int rows = finalCellsY + 1;

        Vector3[] vertices = new Vector3[cols * rows];
        Vector2[] uv = new Vector2[cols * rows];

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

        mf.sharedMesh = mesh;

        Debug.Log(
            $"Cells = {finalCellsX} x {finalCellsY}, " +
            $"Vertices = {mesh.vertexCount}, " +
            $"Triangles = {mesh.triangles.Length / 3}"
        );
    }

    void OnDrawGizmos()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null) return;

        Vector3[] vertices = mf.sharedMesh.vertices;

        int factor = (int)Mathf.Pow(2, subdivisionLevel - 1);

        int finalCellsX = cellsX * factor;
        int finalCellsY = cellsY * factor;

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
                    Gizmos.DrawLine(p, transform.TransformPoint(vertices[right]));
                }

                if (y < rows - 1)
                {
                    int up = x + (y + 1) * cols;
                    Gizmos.DrawLine(p, transform.TransformPoint(vertices[up]));
                }
            }
        }
    }
}