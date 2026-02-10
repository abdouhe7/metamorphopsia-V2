using UnityEngine;

namespace Lessons.L2
{
    // ═══════════════════════════════════════════════════════════════
    // L2 Topic: Subdivision  (Q7)
    //
    //   Subdivision doubles the resolution of a mesh while keeping
    //   its overall shape, by inserting new vertices between every
    //   pair of existing neighbours.
    //
    //   Starting grid: 11×11 vertices  (10×10 cells)
    //   After 1st sub: 21×21 vertices  (20×20 cells)  ← Amsler resolution
    //   After 2nd sub: 41×41 vertices  (40×40 cells)
    //
    //   The key rule:
    //       new_width  = 2 × old_width  − 1
    //       new_height = 2 × old_height − 1
    //
    //   Four passes build each subdivided mesh:
    //     Pass 1 – copy existing vertices to even (x,y) positions
    //     Pass 2 – insert horizontal edge midpoints  (odd x, even y)
    //     Pass 3 – insert vertical   edge midpoints  (even x, odd y)
    //     Pass 4 – insert quad centre points         (odd  x, odd  y)
    // ═══════════════════════════════════════════════════════════════
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class L2_Subdivision : MonoBehaviour
    {
        [Header("World size")]
        public float meshWidth  = 1f;
        public float meshHeight = 1f;

        [Header("Read-only state")]
        [SerializeField] private int _cols;
        [SerializeField] private int _rows;

        private MeshFilter _mf;
        private Mesh       _mesh;

        void Start()
        {
            _mf = GetComponent<MeshFilter>();
            CreateBaseGrid();
        }


        // ─────────────────────────────────────────────────────────────
        // Step 0 – Build the starting 11×11 vertex grid (10×10 cells)
        // ─────────────────────────────────────────────────────────────
        [ContextMenu("1 – Create Base Grid (11×11 vertices)")]
        public void CreateBaseGrid()
        {
            _cols = 11;
            _rows = 11;
            _mesh = L2_MeshConcepts.BuildMesh("Grid_11x11", _cols, _rows, meshWidth, meshHeight, null);
            if (_mf == null) _mf = GetComponent<MeshFilter>();
            _mf.mesh = _mesh;
            Debug.Log("[L2 Subdivision] Base grid: " + _cols + "×" + _rows +
                      " vertices  (" + (_cols-1) + "×" + (_rows-1) + " cells)");
        }


        // ─────────────────────────────────────────────────────────────
        // Step 1+ – Subdivide: each call doubles the resolution
        //           11×11  →  21×21  →  41×41
        // ─────────────────────────────────────────────────────────────
        [ContextMenu("2 – Subdivide (doubles resolution)")]
        public void Subdivide()
        {
            if (_mesh == null)
            {
                Debug.LogWarning("[L2 Subdivision] No mesh found – run CreateBaseGrid first.");
                return;
            }

            int oldCols = _cols;
            int oldRows = _rows;
            int newCols = 2 * oldCols - 1;
            int newRows = 2 * oldRows - 1;

            Vector3[] oldVerts = _mesh.vertices;            // snapshot before rebuild
            Vector3[] newVerts = new Vector3[newCols * newRows];

            // ── Pass 1: copy original vertices to even (x, y) slots ──
            //
            //   Old vertex at (ox, oy)  maps to  new index (ox*2, oy*2)
            //   because every old position now sits at an even coordinate.
            //
            //   old   new (×2 stride)
            //   0,0 → 0,0
            //   1,0 → 2,0
            //   0,1 → 0,2  …
            for (int oy = 0; oy < oldRows; oy++)
            {
                for (int ox = 0; ox < oldCols; ox++)
                {
                    int oldIdx = ox       + oy       * oldCols;
                    int newIdx = (ox * 2) + (oy * 2) * newCols;
                    newVerts[newIdx] = oldVerts[oldIdx];
                }
            }

            // ── Pass 2: horizontal edge midpoints  (odd x, even y) ───
            //   Sit between two existing vertices on the same row.
            for (int ny = 0; ny < newRows; ny += 2)
            {
                for (int nx = 1; nx < newCols - 1; nx += 2)
                {
                    int left  = (nx - 1) + ny * newCols;
                    int right = (nx + 1) + ny * newCols;
                    newVerts[nx + ny * newCols] = (newVerts[left] + newVerts[right]) * 0.5f;
                }
            }

            // ── Pass 3: vertical edge midpoints  (even x, odd y) ─────
            //   Sit between two existing vertices on the same column.
            for (int ny = 1; ny < newRows - 1; ny += 2)
            {
                for (int nx = 0; nx < newCols; nx += 2)
                {
                    int below = nx + (ny - 1) * newCols;
                    int above = nx + (ny + 1) * newCols;
                    newVerts[nx + ny * newCols] = (newVerts[below] + newVerts[above]) * 0.5f;
                }
            }

            // ── Pass 4: quad centre points  (odd x, odd y) ───────────
            //   Average of the four edge-midpoints surrounding this centre.
            for (int ny = 1; ny < newRows - 1; ny += 2)
            {
                for (int nx = 1; nx < newCols - 1; nx += 2)
                {
                    int left  = (nx - 1) + ny       * newCols;
                    int right = (nx + 1) + ny       * newCols;
                    int below = nx       + (ny - 1) * newCols;
                    int above = nx       + (ny + 1) * newCols;
                    newVerts[nx + ny * newCols] =
                        (newVerts[left] + newVerts[right] + newVerts[below] + newVerts[above]) * 0.25f;
                }
            }

            // ── Rebuild mesh with new vertices ────────────────────────
            _cols = newCols;
            _rows = newRows;
            _mesh = L2_MeshConcepts.BuildMesh(
                "Grid_" + _cols + "x" + _rows, _cols, _rows, meshWidth, meshHeight, newVerts);
            _mf.mesh = _mesh;

            Debug.Log("[L2 Subdivision] Subdivided → " + _cols + "×" + _rows +
                      " vertices  (" + (_cols-1) + "×" + (_rows-1) + " cells)");
        }


        // ─────────────────────────────────────────────────────────────
        // Helper: run both subdivisions in sequence for a quick demo
        // ─────────────────────────────────────────────────────────────
        [ContextMenu("3 – Full demo: 11×11 → 21×21 → 41×41")]
        public void FullDemo()
        {
            CreateBaseGrid();   // 11×11
            Subdivide();        // 21×21
            Subdivide();        // 41×41
        }
    }
}
