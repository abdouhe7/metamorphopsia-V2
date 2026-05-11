using UnityEngine;

public class VertexDragController : MonoBehaviour
{
    public Material normalMat;
    public Material hoverMat;
    public Material selectedMat;

    public float selectRadius = 0.15f;
    public float normalScale = 0.08f;
    public float hoverScale = 0.13f;
    public float selectedScale = 0.20f;

    public Vector3 pointOffset = new Vector3(0f, 0f, -0.08f);

    private MeshFilter meshFilter;
    private Mesh mesh;
    private Vector3[] vertices;

    private Camera cam;
    private Plane meshPlane;

    private VertexPoint[] points;

    private int hoveredIndex = -1;
    private int selectedIndex = -1;

    private Vector3 dragOffset;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        vertices = mesh.vertices;

        cam = Camera.main;

        points = GetComponentsInChildren<VertexPoint>();

        SyncAllPointsToVertices();

        Debug.Log("VertexDragController started. Points found: " + points.Length);
    }

    void Update()
    {
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null) return;
        }

        if (points == null || points.Length == 0) return;

        meshPlane = new Plane(transform.forward, transform.position);

        HandleHover();
        HandleClick();
        HandleDrag();
    }

    void HandleHover()
    {
        if (selectedIndex >= 0) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        float bestDistance = float.MaxValue;
        int newHover = -1;

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 pointWorld = points[i].transform.position;

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
            Debug.Log("Mouse clicked. Hovered index = " + hoveredIndex);

            if (hoveredIndex < 0) return;

            selectedIndex = hoveredIndex;
            hoveredIndex = -1;

            SetPointStyle(selectedIndex, selectedMat, selectedScale);

            int vertexIndex = points[selectedIndex].vertexIndex;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (meshPlane.Raycast(ray, out float enter))
            {
                Vector3 hitLocal =
                    transform.InverseTransformPoint(ray.GetPoint(enter));

                dragOffset = vertices[vertexIndex] - hitLocal;
                dragOffset.z = 0f;
            }

            Debug.Log("Selected vertex: " + vertexIndex);
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

        int vertexIndex = points[selectedIndex].vertexIndex;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!meshPlane.Raycast(ray, out float enter))
            return;

        Vector3 hitLocal =
            transform.InverseTransformPoint(ray.GetPoint(enter));

        Vector3 newPos = new Vector3(
            hitLocal.x + dragOffset.x,
            hitLocal.y + dragOffset.y,
            vertices[vertexIndex].z
        );

        vertices[vertexIndex] = newPos;

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        SyncAllPointsToVertices();
        SetPointStyle(selectedIndex, selectedMat, selectedScale);
    }

    void SyncAllPointsToVertices()
    {
        for (int i = 0; i < points.Length; i++)
        {
            int vertexIndex = points[i].vertexIndex;

            if (vertexIndex < 0 || vertexIndex >= vertices.Length)
                continue;

            points[i].transform.localPosition =
                vertices[vertexIndex] + pointOffset;
        }
    }

    void SetPointStyle(int pointArrayIndex, Material mat, float scale)
    {
        if (pointArrayIndex < 0 || pointArrayIndex >= points.Length) return;

        Transform t = points[pointArrayIndex].transform;
        t.localScale = Vector3.one * scale;

        Renderer r = t.GetComponent<Renderer>();
        if (r != null && mat != null)
            r.material = mat;
    }
}