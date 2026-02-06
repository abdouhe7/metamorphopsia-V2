using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomGrid;
using RasterizerCS;

public class Simple2DGrid : BaseAssessmentController
{
    //public static GridDecoration gridDecoration;
    public AmslerGridEyeTracking amslerGridEyeTracking;
    public GameObject[] gridPoints;

    //public ComputeShader rasterizerShader;

    //public Camera gridCamera;
    //static public bool showTexture = false;

    public override void Reset()
    {
        amslerGridEyeTracking.Reset();
        //gridDecoration.DestroyPoints();
        //gridDecoration = null;
    }

    public override void Initialize()
    {
        //InitializeGrid();
        //Rasterizer.Instance().Initilize(rasterizerShader);
        //gridDecoration = new GridDecoration(GetComponent<MeshFilter>().mesh, transform);
        //amslerGridEyeTracking.Initialize(GetPoints());
        base.Initialize();
    }

    //void InitializeGrid()
    //{
    //    GetComponent<MeshFilter>().mesh = GridGeneration.Instance().Initilize(3);
    //    if (GetComponent<MeshFilter>().mesh == null)
    //    {
    //        Debug.Log("Mesh initialization fault.");
    //    }
    //}

    private GameObject[] GetPoints()
    {
        //return gridDecoration.GetPoints();
        return gridPoints;
    }

    //private void Update()
    //{
    //    if(gridDecoration != null)
    //    {
    //        ShaderDecorateUpdate();
    //    }
    //}

    //private void ShaderDecorateUpdate()
    //{
    //    GetComponent<Renderer>().material.SetInt("width", GridGeneration.Instance().GetWidthVerticesNumber());
    //    GetComponent<Renderer>().material.SetInt("height", GridGeneration.Instance().GetHeightVerticesNumber());
    //    GetComponent<Renderer>().material.SetFloat("scale", 0.5f - 0.01f * (float)GridGeneration.Instance().subdivisionLevel);
    //    GetComponent<Renderer>().material.SetFloat("_ShowTexture", 0f);
    //}
}
