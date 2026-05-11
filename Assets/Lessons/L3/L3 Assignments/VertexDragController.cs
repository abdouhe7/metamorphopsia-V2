using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VertexDragController : MonoBehaviour
{
    [Header("Gizmo Points")]
    public float pointRadius = 0.04f;
    public Vector3 pointOffset = new Vector3(0f, 0f, -0.08f);

    [Header("Materials")]
    public Material normalMat;
    public Material hoverMat;
    public Material selectedMat;
    public Material lockedMat;

    [Header("Selection")]
    public float selectRadius = 0.15f;
    public float normalScale = 0.08f;
    public float hoverScale = 0.13f;
    public float selectedScale = 0.20f;

    [Header("Neighbour Radius")]
    public bool enableNeighbourRadius = true;
    public float neighbourRadius = 1.5f;
    public float neighbourStrength = 1.0f;

    [Header("Boundary Lock")]
    public bool lockBoundaryPoints = true;

    private MeshFilter meshFilter;
    private Mesh mesh;
    private Vector3[] vertices;

    private GameObject[] gizmos;
    private bool[] isBoundary;

    private int gridCols;
    private int gridRows;

    private Camera cam;
    private Plane meshPlane;

    private int hoveredIndex = -1;
    private int selectedIndex = -1;
    private Vector3 dragOffset;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        cam = Camera.main;

        mesh = meshFilter.mesh;
        vertices = mesh.vertices;

        DetectGridSize();
        ClassifyBoundaryVertices();
        SpawnVertexGizmos();

        Debug.Log($"Vertices = {vertices.Length}, Cols = {gridCols}, Rows = {gridRows}");
    }

    void Update()
    {
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null) return;
        }

        CheckForMeshRebuild();

        if (gizmos == null || gizmos.Length == 0)
            return;

        meshPlane = new Plane(transform.forward, transform.position);

        HandleHover();
        HandleClick();
        HandleDrag();
    }

    void CheckForMeshRebuild()
    {
        Mesh currentMesh = meshFilter.mesh;

        if (mesh == currentMesh &&
            vertices != null &&
            gizmos != null &&
            currentMesh.vertexCount == gizmos.Length)
            return;

        mesh = currentMesh;
        vertices = mesh.vertices;

        DetectGridSize();
        ClassifyBoundaryVertices();

        DestroyGizmos();
        SpawnVertexGizmos();

        hoveredIndex = -1;
        selectedIndex = -1;

        Debug.Log($"Mesh rebuilt. Vertices = {vertices.Length}, Cols = {gridCols}, Rows = {gridRows}");
    }

    void DetectGridSize()
    {
        float firstY = vertices[0].y;
        gridCols = 0;

        for (int i = 0; i < vertices.Length; i++)
        {
            if (Mathf.Abs(vertices[i].y - firstY) > 0.0001f)
                break;

            gridCols++;
        }

        gridRows = vertices.Length / gridCols;
    }

    void ClassifyBoundaryVertices()
    {
        isBoundary = new bool[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            int x = i % gridCols;
            int y = i / gridCols;

            isBoundary[i] =
                x == 0 ||
                x == gridCols - 1 ||
                y == 0 ||
                y == gridRows - 1;
        }
    }

    void SpawnVertexGizmos()
    {
        gizmos = new GameObject[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            g.name = "V_" + i;
            g.transform.SetParent(transform, false);
            g.transform.localPosition = vertices[i] + pointOffset;
            g.transform.localScale = Vector3.one * normalScale;

            Collider oldCollider = g.GetComponent<Collider>();
            if (oldCollider != null)
                Destroy(oldCollider);

            SphereCollider sc = g.AddComponent<SphereCollider>();
            sc.isTrigger = true;
            sc.radius = 0.5f;

            Renderer r = g.GetComponent<Renderer>();

            if (lockBoundaryPoints && isBoundary[i] && lockedMat != null)
                r.material = lockedMat;
            else if (normalMat != null)
                r.material = normalMat;

            gizmos[i] = g;
        }
    }

    void DestroyGizmos()
    {
        if (gizmos == null) return;

        for (int i = 0; i < gizmos.Length; i++)
        {
            if (gizmos[i] != null)
                Destroy(gizmos[i]);
        }

        gizmos = null;
    }

    bool IsLocked(int index)
    {
        return lockBoundaryPoints && isBoundary[index];
    }

    void HandleHover()
    {
        if (selectedIndex >= 0) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        float bestDistance = float.MaxValue;
        int newHover = -1;

        for (int i = 0; i < gizmos.Length; i++)
        {
            if (IsLocked(i))
                continue;

            Vector3 pointWorld = gizmos[i].transform.position;

            float distance =
                Vector3.Cross(ray.direction, pointWorld - ray.origin).magnitude;

            float forward =
                Vector3.Dot(pointWorld - ray.origin, ray.direction);

            if (distance < selectRadius && forward > 0f && distance < bestDistance)
            {
                bestDistance = distance;
                newHover = i;
            }
        }

        if (hoveredIndex != newHover)
        {
            if (hoveredIndex >= 0)
                SetPointStyle(hoveredIndex, normalMat, normalScale);

            if (newHover >= 0)
                SetPointStyle(newHover, hoverMat, hoverScale);

            hoveredIndex = newHover;
        }
    }

    void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (hoveredIndex < 0)
                return;

            selectedIndex = hoveredIndex;
            hoveredIndex = -1;

            SetPointStyle(selectedIndex, selectedMat, selectedScale);

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (meshPlane.Raycast(ray, out float enter))
            {
                Vector3 hitLocal =
                    transform.InverseTransformPoint(ray.GetPoint(enter));

                dragOffset = vertices[selectedIndex] - hitLocal;
                dragOffset.z = 0f;
            }

            Debug.Log("Selected vertex: " + selectedIndex);
        }

        if (Input.GetMouseButtonUp(0) && selectedIndex >= 0)
        {
            SetPointStyle(selectedIndex, normalMat, normalScale);
            selectedIndex = -1;
        }
    }

    void HandleDrag()
    {
        if (!Input.GetMouseButton(0)) return;
        if (selectedIndex < 0) return;
        if (IsLocked(selectedIndex)) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!meshPlane.Raycast(ray, out float enter))
            return;

        Vector3 hitLocal =
            transform.InverseTransformPoint(ray.GetPoint(enter));

        Vector3 oldPos = vertices[selectedIndex];

        Vector3 newPos = new Vector3(
            hitLocal.x + dragOffset.x,
            hitLocal.y + dragOffset.y,
            oldPos.z
        );

        Vector3 delta = newPos - oldPos;

        vertices[selectedIndex] = newPos;

        if (enableNeighbourRadius)
            MoveNeighbours(selectedIndex, oldPos, delta);

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        SyncAllPointsToVertices();
        SetPointStyle(selectedIndex, selectedMat, selectedScale);
    }

    void MoveNeighbours(int selectedVertexIndex, Vector3 selectedOldPos, Vector3 delta)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            if (i == selectedVertexIndex)
                continue;

            if (IsLocked(i))
                continue;

            float distance = Vector3.Distance(vertices[i], selectedOldPos);

            if (distance > neighbourRadius)
                continue;

            float weight = 1f - (distance / neighbourRadius);

            vertices[i] += delta * weight * neighbourStrength;
        }
    }

    void SyncAllPointsToVertices()
    {
        for (int i = 0; i < gizmos.Length; i++)
        {
            if (gizmos[i] == null) continue;

            gizmos[i].transform.localPosition =
                vertices[i] + pointOffset;
        }
    }

    void SetPointStyle(int index, Material mat, float scale)
    {
        if (index < 0 || index >= gizmos.Length) return;
        if (gizmos[index] == null) return;

        gizmos[index].transform.localScale = Vector3.one * scale;

        Renderer r = gizmos[index].GetComponent<Renderer>();

        if (r != null)
        {
            if (IsLocked(index) && lockedMat != null)
                r.material = lockedMat;
            else if (mat != null)
                r.material = mat;
        }
    }
}