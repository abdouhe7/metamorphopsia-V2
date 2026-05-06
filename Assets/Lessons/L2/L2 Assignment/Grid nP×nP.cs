using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProjectStyleGrid : MonoBehaviour
{
    [Header("Project-style subdivision")]
    public uint subdivisionLevel = 1;
    public uint maxSubdivision = 5;

    [Header("World Size")]
    public float width = 4f;
    public float height = 4f;

    private MeshFilter meshFilter;
    private Mesh mesh;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        CreateGrid();
    }

    void OnValidate()
    {
        if (subdivisionLevel < 1) subdivisionLevel = 1;
        if (subdivisionLevel > maxSubdivision) subdivisionLevel = maxSubdivision;
        if (width <= 0f) width = 1f;
        if (height <= 0f) height = 1f;

        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
            CreateGrid();
    }

    void CreateGrid()
    {
        mesh = new Mesh();
        mesh.name = $"ProjectStyleGrid_Level{subdivisionLevel}";

        int factor = (int)Mathf.Pow(2, subdivisionLevel - 1);

        int verticesWidthNumber = factor * 11 - (factor - 1);
        int verticesHeightNumber = factor * 11 - (factor - 1);

        int verticesNumber = verticesWidthNumber * verticesHeightNumber;

        Vector3[] vertices = new Vector3[verticesNumber];
        Vector2[] uv = new Vector2[verticesNumber];

        float verticesWidthDistance = width / (verticesWidthNumber - 1);
        float verticesHeightDistance = height / (verticesHeightNumber - 1);

        for (int yIndex = 0; yIndex < verticesHeightNumber; yIndex++)
        {
            for (int xIndex = 0; xIndex < verticesWidthNumber; xIndex++)
            {
                int index = xIndex + yIndex * verticesWidthNumber;

                vertices[index] = new Vector3(
                    xIndex * verticesWidthDistance - width / 2f,
                    yIndex * verticesHeightDistance - height / 2f,
                    0f
                );

                uv[index] = new Vector2(
                    (xIndex * verticesWidthDistance) / width,
                    (yIndex * verticesHeightDistance) / height
                );
            }
        }

        int[] triangles = new int[
            (verticesHeightNumber - 1) *
            (verticesWidthNumber - 1) *
            6
        ];

        int t = 0;

        for (int y = 0; y < verticesHeightNumber - 1; y++)
        {
            for (int x = 0; x < verticesWidthNumber - 1; x++)
            {
                triangles[t++] = (y + 0) * verticesWidthNumber + (x + 0);
                triangles[t++] = (y + 1) * verticesWidthNumber + (x + 0);
                triangles[t++] = (y + 1) * verticesWidthNumber + (x + 1);

                triangles[t++] = (y + 0) * verticesWidthNumber + (x + 0);
                triangles[t++] = (y + 1) * verticesWidthNumber + (x + 1);
                triangles[t++] = (y + 0) * verticesWidthNumber + (x + 1);
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;

        Debug.Log(
            $"Subdivision Level = {subdivisionLevel}, " +
            $"Vertices = {mesh.vertexCount}, " +
            $"Cells = {(verticesWidthNumber - 1) * (verticesHeightNumber - 1)}, " +
            $"Triangles = {mesh.triangles.Length / 3}"
        );
    }

    void OnDrawGizmos()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null) return;

        Vector3[] vertices = mf.sharedMesh.vertices;

        int factor = (int)Mathf.Pow(2, subdivisionLevel - 1);
        int verticesWidthNumber = factor * 11 - (factor - 1);
        int verticesHeightNumber = factor * 11 - (factor - 1);

        if (vertices.Length != verticesWidthNumber * verticesHeightNumber) return;

        Gizmos.color = Color.black;

        for (int y = 0; y < verticesHeightNumber; y++)
        {
            for (int x = 0; x < verticesWidthNumber; x++)
            {
                int index = x + y * verticesWidthNumber;
                Vector3 p = transform.TransformPoint(vertices[index]);

                if (x < verticesWidthNumber - 1)
                {
                    int right = (x + 1) + y * verticesWidthNumber;
                    Gizmos.DrawLine(p, transform.TransformPoint(vertices[right]));
                }

                if (y < verticesHeightNumber - 1)
                {
                    int up = x + (y + 1) * verticesWidthNumber;
                    Gizmos.DrawLine(p, transform.TransformPoint(vertices[up]));
                }
            }
        }
    }
}