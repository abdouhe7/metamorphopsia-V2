using UnityEngine;

namespace Lessons.L3
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class L3_VertexInteraction : MonoBehaviour
    {
        [Header("Gizmo")]
        public float gizmoRadius = 0.015f;

        [Header("Materials")]
        public Material matDefault;
        public Material matHover;
        public Material matSelected;
        public Material matLocked;

        [Header("Influence Radius (Q4)")]
        public bool  enableInfluenceRadius = false;
        public float influenceRadius = 0.1f;

        [Header("Boundary Lock (Q17)")]
        public bool lockBoundaryVertices = false;

        [Header("Grid Info (auto-detected)")]
        [SerializeField] private int _cols;
        [SerializeField] private int _rows;

        private MeshFilter   _mf;
        private Mesh         _mesh;
        private Vector3[]    _vertices;      // working copy of mesh.vertices
        private GameObject[] _gizmos;        // sphere per vertex, gizmo[i] = vertex[i]
        private bool[]       _isBoundary;

        private int     _hoveredIndex  = -1;
        private int     _selectedIndex = -1;
        private Vector3 _dragOffset;
        private Camera  _cam;
        private Plane   _meshPlane;

        void Start()
        {
            _mf      = GetComponent<MeshFilter>();
            _cam     = Camera.main;
            _mesh    = _mf.mesh;
            _vertices = _mesh.vertices;
            DetectGridDimensions();
            ClassifyBoundaryVertices();
            SpawnVertexGizmos();
        }

        void Update()
        {
            _meshPlane = new Plane(transform.forward, transform.position);
            CheckForMeshRebuild();
            HandleHover();
            HandleClick();
            HandleDrag();
        }

        // ─────────────────────────────────────────────────────
        // CheckForMeshRebuild
        //   Every frame we compare the current mesh instance and
        //   vertex count against what we spawned gizmos for.
        //   If they differ (subdivision happened, new mesh assigned)
        //   we destroy all existing spheres and spawn fresh ones
        //   at the new vertex positions.
        // ─────────────────────────────────────────────────────
        void CheckForMeshRebuild()
        {
            var currentMesh = _mf.mesh;
            if (currentMesh == _mesh && currentMesh.vertexCount == _gizmos.Length) return;

            // Mesh changed — rebuild everything
            _mesh     = currentMesh;
            _vertices = _mesh.vertices;
            DetectGridDimensions();
            ClassifyBoundaryVertices();
            DestroyGizmos();
            SpawnVertexGizmos();
            _hoveredIndex  = -1;
            _selectedIndex = -1;
            Debug.Log("[L3] Mesh changed — rebuilt " + _gizmos.Length + " gizmos.");
        }

        // ─────────────────────────────────────────────────────
        // DestroyGizmos
        //   Destroys all existing sphere children before a rebuild.
        // ─────────────────────────────────────────────────────
        void DestroyGizmos()
        {
            if (_gizmos == null) return;
            foreach (var g in _gizmos)
                if (g != null) Destroy(g);
            _gizmos = null;
        }

        // ─────────────────────────────────────────────────────
        // DetectGridDimensions
        //   Count vertices in the first row (same Y as vertex[0])
        //   to get cols. rows = total / cols.
        // ─────────────────────────────────────────────────────
        void DetectGridDimensions()
        {
            float firstY = _vertices[0].y;
            _cols = 0;
            foreach (var v in _vertices)
            {
                if (Mathf.Abs(v.y - firstY) > 0.0001f) break;
                _cols++;
            }
            _rows = _vertices.Length / _cols;
        }

        // ─────────────────────────────────────────────────────
        // ClassifyBoundaryVertices  (Q17)
        //   x = i % cols,  y = i / cols
        //   boundary = first or last row/column
        // ─────────────────────────────────────────────────────
        void ClassifyBoundaryVertices()
        {
            _isBoundary = new bool[_vertices.Length];
            for (int i = 0; i < _vertices.Length; i++)
            {
                int x = i % _cols, y = i / _cols;
                _isBoundary[i] = x == 0 || x == _cols-1 || y == 0 || y == _rows-1;
            }
        }

        // ─────────────────────────────────────────────────────
        // SpawnVertexGizmos
        //   One sphere per vertex, placed at vertex local position.
        //   gizmo[i] represents vertex[i].
        // ─────────────────────────────────────────────────────
        void SpawnVertexGizmos()
        {
            _gizmos = new GameObject[_vertices.Length];
            for (int i = 0; i < _vertices.Length; i++)
            {
                var g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                g.name = "V_" + i;
                g.transform.SetParent(transform, false);
                g.transform.localPosition = _vertices[i];
                g.transform.localScale    = Vector3.one * gizmoRadius * 2f;
                Destroy(g.GetComponent<Collider>());
                var sc = g.AddComponent<SphereCollider>();
                sc.isTrigger = true; sc.radius = 0.5f;
                g.GetComponent<MeshRenderer>().material =
                    (lockBoundaryVertices && _isBoundary[i]) ? matLocked : matDefault;
                g.AddComponent<GizmoIndex>().index = i;
                _gizmos[i] = g;
            }
        }

        // ─────────────────────────────────────────────────────
        // HandleHover
        //   Ray from camera through mouse. Closest sphere within
        //   gizmoRadius of the ray turns green.
        // ─────────────────────────────────────────────────────
        void HandleHover()
        {
            if (_gizmos == null) return;
            var ray = _cam.ScreenPointToRay(Input.mousePosition);
            float best = float.MaxValue;
            int newHover = -1;
            for (int i = 0; i < _gizmos.Length; i++)
            {
                if (lockBoundaryVertices && _isBoundary[i]) continue;
                if (i == _selectedIndex) continue;
                var   c    = _gizmos[i].transform.position;
                float dist = Vector3.Cross(ray.direction, c - ray.origin).magnitude;
                float fwd  = Vector3.Dot(c - ray.origin, ray.direction);
                if (dist < gizmoRadius && fwd > 0f && dist < best) { best = dist; newHover = i; }
            }
            if (_hoveredIndex != newHover && _hoveredIndex >= 0)
                SetMat(_hoveredIndex, IsLocked(_hoveredIndex) ? matLocked : matDefault);
            if (newHover >= 0) SetMat(newHover, matHover);
            _hoveredIndex = newHover;
        }

        // ─────────────────────────────────────────────────────
        // HandleClick
        //   MouseDown on hovered sphere → select (blue).
        //   MouseUp → deselect.
        // ─────────────────────────────────────────────────────
        void HandleClick()
        {
            if (_gizmos == null) return;
            if (Input.GetMouseButtonDown(0) && _hoveredIndex >= 0)
            {
                if (_selectedIndex >= 0) SetMat(_selectedIndex, matDefault);
                _selectedIndex = _hoveredIndex;
                _hoveredIndex  = -1;
                SetMat(_selectedIndex, matSelected);
                Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
                if (_meshPlane.Raycast(ray, out float e))
                {
                    var hitLocal = transform.InverseTransformPoint(ray.GetPoint(e));
                    _dragOffset  = _vertices[_selectedIndex] - hitLocal;
                    _dragOffset.z = 0f;
                }
            }
            if (Input.GetMouseButtonUp(0) && _selectedIndex >= 0)
            {
                SetMat(_selectedIndex, matDefault);
                _selectedIndex = -1;
            }
        }

        // ─────────────────────────────────────────────────────
        // HandleDrag
        //   Mouse moves → compute new local XY position.
        //   Write it into _vertices[selectedIndex].
        //   Move the sphere to match.
        //   Apply falloff to neighbours.
        //   Upload _vertices to mesh.
        //   Sphere position = vertex position. Always.
        // ─────────────────────────────────────────────────────
        void HandleDrag()
        {
            if (_gizmos == null || !Input.GetMouseButton(0) || _selectedIndex < 0) return;
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (!_meshPlane.Raycast(ray, out float enter)) return;

            var hit    = transform.InverseTransformPoint(ray.GetPoint(enter));
            var newPos = new Vector3(hit.x + _dragOffset.x, hit.y + _dragOffset.y,
                                     _vertices[_selectedIndex].z);
            var delta  = newPos - _vertices[_selectedIndex];

            // 1. Update the vertex
            _vertices[_selectedIndex] = newPos;
            // 2. Move the sphere to the new vertex position
            _gizmos[_selectedIndex].transform.localPosition = newPos;

            // 3. Pull neighbours
            if (enableInfluenceRadius)
                PullNeighbours(_selectedIndex, delta);

            // 4. Push all vertices to the mesh
            _mesh.vertices = _vertices;
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }

        // ─────────────────────────────────────────────────────
        // PullNeighbours  (Q4)
        //   weight = 1 - (dist / influenceRadius)
        //   Each neighbour vertex moves by delta * weight.
        //   Its sphere moves to match.
        // ─────────────────────────────────────────────────────
        void PullNeighbours(int idx, Vector3 delta)
        {
            for (int i = 0; i < _vertices.Length; i++)
            {
                if (i == idx) continue;
                if (lockBoundaryVertices && _isBoundary[i]) continue;
                float dist = Vector3.Distance(_vertices[i], _vertices[idx]);
                if (dist >= influenceRadius) continue;
                float w = 1f - dist / influenceRadius;
                var moved = _vertices[i] + delta * w;
                moved.z = _vertices[i].z;
                _vertices[i] = moved;
                // sphere follows vertex
                _gizmos[i].transform.localPosition = moved;
            }
        }

        void SetMat(int i, Material m)
        {
            if (m == null || _gizmos == null || i < 0 || i >= _gizmos.Length) return;
            var mr = _gizmos[i].GetComponent<MeshRenderer>();
            if (mr) mr.material = m;
        }

        bool IsLocked(int i) => lockBoundaryVertices && _isBoundary[i];

        public class GizmoIndex : MonoBehaviour { public int index; }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (_selectedIndex < 0 || _gizmos == null || !enableInfluenceRadius) return;
            Gizmos.color = new Color(1f,1f,0f,0.3f);
            Gizmos.DrawWireSphere(_gizmos[_selectedIndex].transform.position, influenceRadius);
        }
#endif
    }
}
