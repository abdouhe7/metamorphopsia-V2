using System.Diagnostics;
using UnityEngine;

namespace Lessons.L4
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class L4_GenerateGrid : MonoBehaviour
    {
        [Header("Compute Shader")]
        public ComputeShader gridGenerationShader;

        [Header("Grid Size")]
        public int columns = 101;
        public int rows = 101;

        [Header("World Size")]
        public float meshWidth = 2f;
        public float meshHeight = 2f;

        [Header("Heavy Computation (to show GPU advantage)")]
        [Tooltip("Each vertex runs this many sine-wave iterations. Higher = more work, GPU wins bigger.")]
        public int iterationsPerVertex = 10000;

        [ContextMenu("Generate Grid (GPU)")]
        public void GenerateGrid()
        {
            if (gridGenerationShader == null)
            {
                UnityEngine.Debug.LogError("[L4 GPU Grid] No compute shader assigned!");
                return;
            }

            int totalVertices = columns * rows;
            int totalQuads = (columns - 1) * (rows - 1);
            int totalTriangles = totalQuads * 6;

            // --- GPU buffers (names match compute shader AND CPU arrays) ---
            ComputeBuffer verticesBuffer = new ComputeBuffer(totalVertices, 3 * sizeof(float));
            ComputeBuffer uvsBuffer = new ComputeBuffer(totalVertices, 2 * sizeof(float));
            ComputeBuffer trianglesBuffer = new ComputeBuffer(totalTriangles, sizeof(uint));

            // --------------------------------------------------
            // Kernel 1: Vertex positions + UVs + heavy computation
            // --------------------------------------------------
            int vertexKernel = gridGenerationShader.FindKernel("VertexKernel");
            gridGenerationShader.SetBuffer(vertexKernel, "vertices", verticesBuffer);
            gridGenerationShader.SetBuffer(vertexKernel, "uvs", uvsBuffer);
            gridGenerationShader.SetInt("columns", columns);
            gridGenerationShader.SetInt("rows", rows);
            gridGenerationShader.SetFloat("meshWidth", meshWidth);
            gridGenerationShader.SetFloat("meshHeight", meshHeight);
            gridGenerationShader.SetInt("iterationsPerVertex", iterationsPerVertex);

            // ================================================================
            // GPU Timing — start BEFORE Dispatch, stop AFTER GetData
            // ================================================================
            Stopwatch sw = Stopwatch.StartNew();

            int vertexGroups = Mathf.CeilToInt(totalVertices / 64f);
            gridGenerationShader.Dispatch(2, vertexGroups, 1, 1);

            // --------------------------------------------------
            // Kernel 2: Triangle indices
            // --------------------------------------------------
            int triangleKernel = gridGenerationShader.FindKernel("TriangleKernel");
            gridGenerationShader.SetBuffer(triangleKernel, "triangles", trianglesBuffer);

            int triGroups = Mathf.CeilToInt(totalQuads / 64f);
            gridGenerationShader.Dispatch(triangleKernel, triGroups, 1, 1);

            // --- Read data back from GPU (BLOCKS until GPU finishes) ---
            Vector3[] vertices = new Vector3[totalVertices];
            Vector2[] uvs = new Vector2[totalVertices];
            int[] triangles = new int[totalTriangles];

            verticesBuffer.GetData(vertices);
            uvsBuffer.GetData(uvs);
            trianglesBuffer.GetData(triangles);

            // --- Build Unity Mesh from read-back data ---
            Mesh mesh = new Mesh { name = "L4_Grid_GPU" };
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();

            GetComponent<MeshFilter>().mesh = mesh;

            sw.Stop();

            // --- Release GPU resources ---
            verticesBuffer.Release();
            uvsBuffer.Release();
            trianglesBuffer.Release();

            UnityEngine.Debug.Log(
                $"[L4 GPU Grid] {columns}x{rows} ({totalVertices} verts, {iterationsPerVertex} iter/vert)" +
                $"  TOTAL: {sw.Elapsed.TotalMilliseconds:F4} ms"
            );
        }

        void Start()
        {
            GenerateGrid();
        }
    }
}
