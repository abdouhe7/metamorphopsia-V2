using System.Diagnostics;
using UnityEngine;

namespace Lessons.L3
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class L3_GenerateGrid : MonoBehaviour
    {
        [Header("Grid Size")]
        public int columns = 101;
        public int rows = 101;

        [Header("World Size")]
        public float meshWidth = 2f;
        public float meshHeight = 2f;

        [Header("Heavy Computation (to show GPU advantage)")]
        [Tooltip("Each vertex runs this many sine-wave iterations. Higher = more work, GPU wins bigger.")]
        public int iterationsPerVertex = 10000;

        [ContextMenu("Generate Grid (CPU)")]
        public void GenerateGrid()
        {
            Stopwatch sw = Stopwatch.StartNew();

            int total = columns * rows;

            Vector3[] vertices = new Vector3[total];
            Vector2[] uvs = new Vector2[total];

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    int index = x + y * columns;
                    float u = (float)x / (columns - 1);
                    float v = (float)y / (rows - 1);

                    // Heavy per-vertex computation — IDENTICAL to GPU version
                    // Fourier series: sum sin(i*2π*u) * cos(i*2π*v) / i
                    // Clean saddle-wave pattern, converges slowly (1/i decay).
                    float height = 0f;
                    for (int i = 1; i <= iterationsPerVertex; i++)
                    {
                        float fi = (float)i;
                        height += Mathf.Sin(fi * 6.2831853f * u) * Mathf.Cos(fi * 6.2831853f * v) / fi;
                    }
                    height *= 0.05f;

                    vertices[index] = new Vector3(
                        (u - 0.5f) * meshWidth,
                        (v - 0.5f) * meshHeight,
                        height
                    );
                    uvs[index] = new Vector2(u, v);
                }
            }

            int[] triangles = new int[(columns - 1) * (rows - 1) * 6];
            int t = 0;
            for (int y = 0; y < rows - 1; y++)
            {
                for (int x = 0; x < columns - 1; x++)
                {
                    int bl = x + y * columns;
                    int br = (x + 1) + y * columns;
                    int tl = x + (y + 1) * columns;
                    int tr = (x + 1) + (y + 1) * columns;

                    triangles[t++] = bl; triangles[t++] = tl; triangles[t++] = tr;
                    triangles[t++] = bl; triangles[t++] = tr; triangles[t++] = br;
                }
            }

            Mesh mesh = new Mesh { name = "L3_Grid_CPU" };
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();

            GetComponent<MeshFilter>().mesh = mesh;

            sw.Stop();

            UnityEngine.Debug.Log(
                $"[L3 CPU Grid] {columns}x{rows} ({total} verts, {iterationsPerVertex} iter/vert)" +
                $"  TOTAL: {sw.Elapsed.TotalMilliseconds:F4} ms"
            );
        }

        void Start()
        {
            GenerateGrid();
        }
    }
}
