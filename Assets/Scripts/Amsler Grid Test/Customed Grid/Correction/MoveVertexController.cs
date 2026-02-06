using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CustomGrid;
using UnityEngine.Experimental.GlobalIllumination;

public class MoveVertexController : MonoBehaviour
{
    public static UnityEvent ChangeGrid = new UnityEvent();
    public static UnityEvent ShowDotsUI = new UnityEvent();
    public static UnityEvent ResetSelectedPointPosition = new UnityEvent();

    static private int selectedRowLine;
    static private int selectedColumnLine;
    static private int selectedPoint;

    private bool selectionState = false;
    private int lastHoveredPoint = -1;

    private Vector3[] vertices;
    static private int width, height;

    public float speed;

    static public bool showGrid = true;
    static public bool showLines = true;
    MaterialPropertyBlock materialBlock;

    bool Boundary(int index, Vector2 direction)
    {
        Vector3 xVector = Vector3.zero, yVector = Vector3.zero;
        if (direction.x > 0 && direction.y > 0)
        {
            yVector = vertices[index + GridGeneration.Instance().GetWidthVerticesNumber()] - vertices[index];
            xVector = vertices[index + 1] - vertices[index];
        }
        else if (direction.x > 0 && direction.y < 0)
        {
            yVector = vertices[index - GridGeneration.Instance().GetWidthVerticesNumber()] - vertices[index];
            xVector = vertices[index + 1] - vertices[index];
        }
        else if (direction.x < 0 && direction.y > 0)
        {
            yVector = vertices[index + GridGeneration.Instance().GetWidthVerticesNumber()] - vertices[index];
            xVector = vertices[index - 1] - vertices[index];
        }
        else if (direction.x < 0 && direction.y < 0)
        {
            yVector = vertices[index - GridGeneration.Instance().GetWidthVerticesNumber()] - vertices[index];
            xVector = vertices[index - 1] - vertices[index];
        }

        if (Vector3.Dot(xVector.normalized, yVector.normalized) <= -0.98)
            return false;

        return true;
    }

    bool IsBoundaryPoint(int index)
    {
        int width = GridGeneration.Instance().GetWidthVerticesNumber();
        int height = GridGeneration.Instance().GetHeightVerticesNumber();

        // Top boundary
        if (index < width)
            return true;

        // Bottom boundary
        if (index >= (width * (height - 1)))
            return true;

        // Left boundary
        if (index % width == 0)
            return true;

        // Right boundary
        if ((index + 1) % width == 0)
            return true;

        return false;
    }


    bool MovePoint(float speed, float radius)
    {
        Vector2 direction = ControllerOutput.rightaxisDirection;

        int index = selectedPoint;
        if (direction != Vector2.zero && Boundary(index, direction))
        {
            vertices[index].x += speed * direction.x;
            vertices[index].y += speed * direction.y;

            Vector2 selectedPointPos = vertices[index];

            // Logic for neighboring point movement
            for (int i = 0; i < vertices.Length; i++)
            {
                if (i == index) continue;

                Vector2 pointPos = vertices[i];
                float distance = Vector2.Distance(selectedPointPos, pointPos);

                if (distance <= radius)
                {
                    float influenceFactor = 1 - (distance / radius);
                    vertices[i].x += speed * direction.x * influenceFactor;
                    vertices[i].y += speed * direction.y * influenceFactor;
                }
            }

            return true;
        }

        return false;
    }

    bool MovePointUsingRaycast(float speed, float radius)
    {
        int layer = transform.gameObject.layer;
        LayerMask layerMask = 1 << layer;

        Ray ray = ControllerOutput.controllerRay;
        RaycastHit hit;

        if (selectionState && Physics.Raycast(ray, out hit, 20f, layerMask))
        {
            Transform gridTransform = transform;
            Vector3 localHitPoint3D = gridTransform.InverseTransformPoint(hit.point);
            Vector2 localHitPoint = new Vector2(localHitPoint3D.x, localHitPoint3D.y);

            int index = selectedPoint;

            Vector2 oldSelectedPointPos = vertices[index];

            vertices[index].x = localHitPoint.x;
            vertices[index].y = localHitPoint.y;

            Vector2 newSelectedPointPos = vertices[index];

            Vector2 displacement = newSelectedPointPos - oldSelectedPointPos;

            // Logic for neighboring point movement
            for (int i = 0; i < vertices.Length; i++)
            {
                if (i == index) continue;

                // Skip boundary points
                if (IsBoundaryPoint(i))
                {
                    continue;
                }

                Vector2 pointPos = vertices[i];
                float distance = Vector2.Distance(oldSelectedPointPos, pointPos);

                if (distance <= radius)
                {
                    float influenceFactor = 1 - (distance / radius);

                    vertices[i].x += displacement.x * influenceFactor;
                    vertices[i].y += displacement.y * influenceFactor;
                }
            }

            return true;
        }

        return false;
    }

    void ChoosePoint()
    {
        Vector2 direction = ControllerOutput.rightaxisDirection;

        float threshold = 0.6f;

        if (direction == Vector2.zero)
        {
            selectionState = false;
        }
        if (direction.x >= threshold && !selectionState)
        {
            selectedColumnLine = (selectedColumnLine >= width - 2 ? selectedColumnLine : selectedColumnLine + 1);
            selectionState = true;
        }
        else if (direction.x <= -threshold && !selectionState)
        {
            selectedColumnLine = (selectedColumnLine <= 1 ? selectedColumnLine : selectedColumnLine - 1);
            selectionState = true;
        }
        else if (direction.y >= threshold && !selectionState)
        {
            selectedRowLine = (selectedRowLine >= height - 2 ? selectedRowLine : selectedRowLine + 1);
            selectionState = true;
        }
        else if (direction.y <= -threshold && !selectionState)
        {
            selectedRowLine = (selectedRowLine <= 1 ? selectedRowLine : selectedRowLine - 1);
            selectionState = true;
        }

        selectedPoint = selectedColumnLine + width * selectedRowLine;
    }

    void ChoosePointWithRaycast()
    {
        var pointObject = ControllerOutput.hitObjectRight;
        var triggerPressed = ControllerOutput.pressPrimaryButton;

        var triggerPressedDown = ControllerOutput.pressDownPrimaryButton;

        if (pointObject != null && pointObject.CompareTag("Point"))
        {
            var pointData = pointObject.GetComponent<PointData>();
            if(pointData != null)
            {
                if (lastHoveredPoint != pointData.index)
                {
                    lastHoveredPoint = pointData.index;
                }

                if (triggerPressed)
                {
                    selectionState = true;
                    selectedPoint = pointData.index;
                    selectedRowLine = selectedPoint / width;
                    selectedColumnLine = selectedPoint % width;
                }
                else
                {
                    selectionState = false;
                }

                if(triggerPressedDown && selectionState)
                {
                    pointData.PlayPointSelection();
                }
            }
        }
    }

    public static void ResetPoint()
    {
        selectedPoint = 0;
    }

    void ShowDots()
    {
        if (showGrid)
        {
            for (int i = 0; i < GameObject.Find("Points").transform.childCount; ++i)
            {
                GameObject.Find("Points").transform.GetChild(i).gameObject.SetActive(true);
                GameObject.Find("Grid").GetComponent<Renderer>().material.SetFloat("_ShowGrid", 1.0f);
            }
        }
        else
        {
            for (int i = 0; i < GameObject.Find("Points").transform.childCount; ++i)
            {
                GameObject.Find("Points").transform.GetChild(i).gameObject.SetActive(false);
                GameObject.Find("Grid").GetComponent<Renderer>().material.SetFloat("_ShowGrid", 0.0f);
            }
        }
    }

    private void SetLineColor()
    {
        Vector4 lineColor = new Vector4(0, 0, 0, showLines ? 1 : 0);

        GetComponent<Renderer>().material.SetInt("XSelected", showLines ? selectedColumnLine : 0);
        GetComponent<Renderer>().material.SetInt("YSelected", showLines ? selectedRowLine : 0);
        GetComponent<Renderer>().material.SetVector("restColor", lineColor);

        if (!showGrid)
            GameObject.Find("Points").transform.GetChild(selectedPoint).gameObject.SetActive(true);

    }

    void RefreshDotAtOneDotMode()
    {
        if (!showGrid)
            GameObject.Find("Points").transform.GetChild(selectedPoint).gameObject.SetActive(false);
    }

    static public void Initilize()
    {
        width = GridGeneration.Instance().GetWidthVerticesNumber();
        height = GridGeneration.Instance().GetHeightVerticesNumber();

        selectedRowLine = height / 2; // 1;
        selectedColumnLine = width / 2; // 1;
        selectedPoint = selectedColumnLine + width * selectedRowLine;
    }

    private void Awake()
    {
        speed = 0.8f;

        ChangeGrid.AddListener(ChangeStructure);
        ShowDotsUI.AddListener(ShowDots);
        ResetSelectedPointPosition.AddListener(ResetState);

        materialBlock = new MaterialPropertyBlock();
    }

    private void OnDisable()
    {
        materialBlock.Clear();
    }

    private void Start()
    {
        Initilize();
    }

    private void ResetState()
    {
        selectedRowLine *= 2;
        selectedColumnLine *= 2;

        speed *= 0.8f;
    }

    private void ChangeStructure()
    {
        float moveSpeed = Time.deltaTime * speed;
        float radius = 5f;


        width = GridGeneration.Instance().GetWidthVerticesNumber();
        height = GridGeneration.Instance().GetHeightVerticesNumber();

        vertices = GetComponent<MeshFilter>().mesh.vertices;

        RefreshDotAtOneDotMode();
        //ChoosePoint();
        ChoosePointWithRaycast();
        if (MovePointUsingRaycast(moveSpeed, radius))
        {
            GridDecoration.changed = true;
            GetComponent<MeshFilter>().mesh.SetVertices(vertices);
        }

        SetLineColor();
    }
}