using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using CustomGrid;
using RasterizerCS;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridManager : MonoBehaviour
{
    public static GridDecoration gridDecoration;
    public static bool showTexture;
    public static bool showGrid;

    public string instructions;
    public Camera gridCamera;
    public ComputeShader rasterizerShader;

    private MeshRenderer meshRenderer;

    public string GetInstructions()
    {
        return instructions;
    }

    public void ConstructAndDeactivate()
    {
        InitializeGrid();
        Rasterizer.Instance().Initilize(rasterizerShader);
        gridDecoration = new GridDecoration(GetComponent<MeshFilter>().mesh, transform);
    }

    public void InitializeGrid()
    {
        GetComponent<MeshFilter>().mesh = GridGeneration.Instance().Initilize(2);
        if (GetComponent<MeshFilter>().mesh == null)
        {
            Debug.Log("Mesh initialization fault.");
        }

        meshRenderer = GetComponent<MeshRenderer>();

        //SetGridPosition();
    }

    public void SetMeshState(bool state)
    {
        meshRenderer.enabled = state;
    }

    void SetGridPosition()
    {
        var width = (float)SimpleGameManager.Instance.width / 100.0f;
        var height = (float)SimpleGameManager.Instance.height / 100.0f;
        var screenRatio = height / width;

        var gridOffset = SimpleGameManager.Instance.offset;
        var newPosition = transform.localPosition;
        newPosition.z = gridOffset + screenRatio * gridOffset;
        transform.localPosition = newPosition;
    }

    public void Subdivide()
    {
        Mesh subdivisionMesh = GridGeneration.Instance().Subdivision(GetComponent<MeshFilter>().mesh);
        if (subdivisionMesh != null)
            GetComponent<MeshFilter>().mesh = subdivisionMesh;
        Reconstruct();
    }

    public void Reconstruct()
    {
        if (!gridDecoration.Reconstruct(GetComponent<MeshFilter>().mesh, transform))
        {
            Debug.Log("Can't subdivide the grid, the vertices number is same to previous.");
        }
    }

    private void ShaderDecorateUpdate()
    {
        GetComponent<Renderer>().material.SetInt("width", GridGeneration.Instance().GetWidthVerticesNumber());
        GetComponent<Renderer>().material.SetInt("height", GridGeneration.Instance().GetHeightVerticesNumber());
        GetComponent<Renderer>().material.SetFloat("scale", 0.5f - 0.01f * (float)GridGeneration.Instance().subdivisionLevel);
        
        if(showTexture)
            GetComponent<Renderer>().material.SetFloat("_ShowTexture", 1f);
        else
            GetComponent<Renderer>().material.SetFloat("_ShowTexture", 0f);

        if(showGrid)
            GetComponent<Renderer>().material.SetFloat("_ShowGrid", 1f);
        else
            GetComponent<Renderer>().material.SetFloat("_ShowGrid", 0f);
    }

    private void Update()
    {
        OnGridUpdate();
    }

    void OnGridUpdate()
    {
        //if (ControllerOutput.pressPrimaryButton)
        //{
        //    Subdivide();
        //}

        MoveVertexController.ChangeGrid.Invoke();

        GenerateBoard.UVTex = Rasterizer.Instance().Refresh(GetComponent<MeshFilter>().mesh, transform, gridCamera);

        gridDecoration.Update(GetComponent<MeshFilter>().mesh, transform);
        ShaderDecorateUpdate();
    }

    public void DestroyGrid()
    {
        Destroy(GetComponent<MeshFilter>().mesh);
        GetComponent<MeshFilter>().mesh = null;
        gridDecoration.DestroyPoints();
        gridDecoration.DestroyParent();
    }
}