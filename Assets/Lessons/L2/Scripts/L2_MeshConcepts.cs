using UnityEngine;

namespace Lessons.L2
{
    // ═══════════════════════════════════════════════════════════════
    // L2 Topic: What is a Mesh  (Q8)  +  Vertex Index Formula  (Q3)
    //           + Creating Amsler Grid 20×20 (Q3)
    //
    //   A Mesh has three required arrays:
    //     • vertices  – Vector3[] – 3D positions in local space
    //     • triangles – int[]     – groups of 3 indices, each group is one triangle
    //     • uv        – Vector2[] – texture coordinate per vertex (range 0..1)
    //
    //   The index formula turns a 2D (x,y) grid address into a 1D array index:
    //
    //       index = x + y * width        (width = number of vertices per row)
    //
    //   All three demos below use exactly the same algorithm — only the
    //   grid size (cols × rows) changes.
    // ═══════════════════════════════════════════════════════════════
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class L2_MeshConcepts : MonoBehaviour
    {
        public enum Demo { StaticQuad, CalculatedQuad, AmslerGrid20x20 }

        [Header("Which demo to run on Start")]
        public Demo demo = Demo.StaticQuad;

        [Header("World size (used by AmslerGrid demo)")]
        public float meshWidth  = 1f;
        public float meshHeight = 1f;
        
        
        public int gridXNumber  = 20;
        public int gridYNumber  = 20;

        public int Subdivistion = 1;

        [Header("Scene-view vertex labels (editor only)")]
        public bool showVertexLabels = true;

        private Mesh       _mesh;
        public MeshFilter _mf;

        // ── Unity lifecycle ──────────────────────────────────────────
        void Start()
        {
            _mf = GetComponent<MeshFilter>();
            RunDemo();
        }

        [ContextMenu("Run Demo")]
        void RunDemo()
        {
            switch (demo)
            {
                case Demo.StaticQuad:      CreateStaticQuad();      break;
                case Demo.CalculatedQuad:  CreateCalculatedQuad();  break;
                case Demo.AmslerGrid20x20: CreateAmslerGrid(); break;
            }
        }

        [ContextMenu("Subdivision")]

        void Subdivision()
        {
            int cols = gridXNumber + 1;
            int rows = gridYNumber + 1;

            _mesh = BuildMesh("AmslerGrid20x20", cols *Subdivistion , rows*Subdivistion, meshWidth, meshHeight, null);
            _mf.mesh = _mesh;
            Debug.Log("[L2] AmslerGrid20x20 – vertices: " + _mesh.vertexCount +
                      "  quads: " + ((cols - 1) * (rows - 1)) +
                      "  triangles: " + (_mesh.triangles.Length / 3));
            
        }
        // ═══════════════════════════════════════════════════════════
        // DEMO 1  —  Static / hardcoded quad
        //
        //   Shows every array explicitly so every number is visible.
        //   Vertex layout (viewed from front):
        //
        //        3 ───── 2
        //        |  \ B  |
        //        | A \   |
        //        0 ───── 1
        //
        //   Triangle A: 0 → 3 → 2   (counter-clockwise when winding is CW)
        //   Triangle B: 0 → 2 → 1
        // ═══════════════════════════════════════════════════════════
        [ContextMenu("1 – Create Static Quad")]

        // public int Subdivistion1
        // {
        //     get => Subdivistion;
        //     set => Subdivistion = value;
        // }

        public void CreateStaticQuad()
        {
            _mesh = new Mesh { name = "StaticQuad" };

            // ── Vertices: 4 corners ─────────────────────────────────
            _mesh.vertices = new Vector3[]
            {
                new Vector3(-1f, -1f, 0),   // index 0 : bottom-left
                new Vector3( 1f, -1f, 0),   // index 1 : bottom-right
                new Vector3( 1f,  1f, 0),   // index 2 : top-right
                new Vector3(-1f,  1f, 0),   // index 3 : top-left
                
            };

            // ── Triangles: 2 × 3 indices ────────────────────────────
            _mesh.triangles = new int[]
            {
                0, 3, 2,   // Triangle A
                0, 2, 1,   // Triangle B
            };

            // ── UVs: texture coordinates 0..1 per vertex ────────────
            _mesh.uv = new Vector2[]
            {
                new Vector2(0, 0),   // vertex 0
                new Vector2(1, 0),   // vertex 1
                new Vector2(1, 1),   // vertex 2
                new Vector2(0, 1),   // vertex 3
            };

            _mesh.RecalculateNormals();
            _mf.mesh = _mesh;
            Debug.Log("[L2] StaticQuad – vertices: 4  triangles: 2");
        }


        // ═══════════════════════════════════════════════════════════
        // DEMO 2  —  Calculated quad  (same result, procedural code)
        //
        //   index = x + y * cols
        //
        //   For cols = 2, rows = 2:
        //
        //     y=1 →  2 (0+1*2)   3 (1+1*2)
        //     y=0 →  0 (0+0*2)   1 (1+0*2)
        //              x=0           x=1
        //
        //   Cell triangles use the 4 corner indices of each quad:
        //     bl = (x)   + (y)   * cols
        //     br = (x+1) + (y)   * cols
        //     tl = (x)   + (y+1) * cols
        //     tr = (x+1) + (y+1) * cols
        // ═══════════════════════════════════════════════════════════
        [ContextMenu("2 – Create Calculated Quad")]
        public void CreateCalculatedQuad()
        {
            const int cols = 2;   // vertices along X  (= 1 cell wide)
            const int rows = 2;   // vertices along Y  (= 1 cell tall)

            _mesh = BuildMesh("CalculatedQuad", cols, rows, 1f, 1f, null);
            _mf.mesh = _mesh;
            Debug.Log("[L2] CalculatedQuad – vertices: " + _mesh.vertexCount +
                      "  triangles: " + (_mesh.triangles.Length / 3));
        }


        // ═══════════════════════════════════════════════════════════
        // DEMO 3  —  Amsler Grid 20×20
        //
        //   Exactly the same algorithm as Demo 2, scaled to:
        //     cols = 21  (20 cells → 21 vertex columns)
        //     rows = 21  (20 cells → 21 vertex rows)
        //
        //   Result: 441 vertices, 400 quads, 800 triangles.
        //   index formula: index = x + y * 21
        // ═══════════════════════════════════════════════════════════
        [ContextMenu("3 – Create Amsler Grid 20×20")]
        public void CreateAmslerGrid()
        {
            int cols = gridXNumber + 1;
            int rows = gridYNumber + 1;

            _mesh = BuildMesh("AmslerGrid20x20", cols, rows, meshWidth, meshHeight, null);
            _mf.mesh = _mesh;
            Debug.Log("[L2] AmslerGrid20x20 – vertices: " + _mesh.vertexCount +
                      "  quads: " + ((cols - 1) * (rows - 1)) +
                      "  triangles: " + (_mesh.triangles.Length / 3));
        }


        // ─────────────────────────────────────────────────────────────
        // Shared mesh builder used by all three demos
        //
        //   overrideVerts: pass pre-computed positions (e.g. after
        //   subdivision). Pass null to compute a flat evenly-spaced grid.
        // ─────────────────────────────────────────────────────────────
        internal static Mesh BuildMesh(string meshName, int cols, int rows,
                                       float w, float h, Vector3[] overrideVerts)
        {
            Mesh m      = new Mesh { name = meshName };
            int  total  = cols * rows;

            Vector3[] vertices  = (overrideVerts != null) ? overrideVerts : new Vector3[total];
            Vector2[] uvs       = new Vector2[total];

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int   index = x + y * cols;         // ← index formula
                    float u     = (float)x / (cols - 1);
                    float v     = (float)y / (rows - 1);

                    if (overrideVerts == null)
                        vertices[index] = new Vector3((u - 0.5f) * w, (v - 0.5f) * h, 0);

                    uvs[index] = new Vector2(u, v);
                }
            }

            // Two triangles per quad cell
            int[] triangles = new int[(cols - 1) * (rows - 1) * 6];
            int   t         = 0;

            for (int y = 0; y < rows - 1; y++)
            {
                for (int x = 0; x < cols - 1; x++)
                {
                    int bl = x     + y       * cols;
                    int br = (x+1) + y       * cols;
                    int tl = x     + (y + 1) * cols;
                    int tr = (x+1) + (y + 1) * cols;

                    triangles[t++] = bl; triangles[t++] = tl; triangles[t++] = tr;
                    triangles[t++] = bl; triangles[t++] = tr; triangles[t++] = br;
                }
            }

            m.vertices  = vertices;
            m.triangles = triangles;
            m.uv        = uvs;
            m.RecalculateNormals();
            return m;
        }


        // ── Editor-only: draw vertex index labels in Scene view ──────
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!showVertexLabels || _mesh == null) return;

            // Only label small meshes to avoid clutter
            if (_mesh.vertexCount > 50) return;

            UnityEditor.Handles.color = Color.yellow;
            Vector3[] verts = _mesh.vertices;
            for (int i = 0; i < verts.Length; i++)
            {
                Vector3 world = transform.TransformPoint(verts[i]);
                UnityEditor.Handles.Label(world + Vector3.up * 0.04f, i.ToString());
            }
        }
#endif
    }
}
