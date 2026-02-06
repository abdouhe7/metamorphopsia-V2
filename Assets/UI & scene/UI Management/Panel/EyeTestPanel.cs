using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using CustomGrid;
using System;
using System.Collections;

public class SampleSavePanel : BasePanel
{
    static readonly string path = "Panels/GridPanel";
    public SampleSavePanel() : base(new UIType(path)) { }
    private StereoTargetEyeMask currentTarget;

    public override void OnAwake()
    {
        Toggle[] toggles = new Toggle[3];

        toggles[0] = ui_tool.GetOrAddComponentInChildren<Toggle>("LeftEye");
        toggles[1] = ui_tool.GetOrAddComponentInChildren<Toggle>("RightEye");
        toggles[2] = ui_tool.GetOrAddComponentInChildren<Toggle>("BothEyes");

        toggles[0].isOn = false;
        toggles[1].isOn = false;
        toggles[2].isOn = false;

        currentTarget = SaveAndLoad.LoadCurrentTargetUI();

        if (currentTarget != StereoTargetEyeMask.None)
            toggles[(int)currentTarget - 1].isOn = true;
        else
            toggles[2].isOn = true;

        toggles[0].onValueChanged.AddListener((bool isOn) =>
        {
            if (isOn)
            {
                toggles[1].isOn = false;
                toggles[2].isOn = false;

                currentTarget = StereoTargetEyeMask.Left;
                SaveAndLoad.SaveUI(currentTarget);
            }
            else if (!toggles[1].isOn & !toggles[2].isOn)
            {
                toggles[0].isOn = true;
            }
        });

        toggles[1].onValueChanged.AddListener((bool isOn) =>
        {
            if (isOn)
            {
                toggles[0].isOn = false;
                toggles[2].isOn = false;

                currentTarget = StereoTargetEyeMask.Right;
                SaveAndLoad.SaveUI(currentTarget);
            }
            else if (!toggles[0].isOn & !toggles[2].isOn)
            {
                toggles[1].isOn = true;
            }
        });

        toggles[2].onValueChanged.AddListener((bool isOn) =>
        {
            if (isOn)
            {
                toggles[0].isOn = false;
                toggles[1].isOn = false;

                currentTarget = StereoTargetEyeMask.Both;
                SaveAndLoad.SaveUI(currentTarget);
            }
            else if (!toggles[0].isOn & !toggles[1].isOn)
            {
                toggles[2].isOn = true;
            }
        });

        ui_tool.GetOrAddComponentInChildren<Button>("Save").onClick.AddListener(() =>
        {
            Mesh storedMesh = GameObject.Find("Grid").GetComponent<MeshFilter>().mesh;

            if (toggles[0].isOn)
            {
                SaveAndLoad.Save(storedMesh, GenerateBoard.UVTex, "LeftEyeSample");
            }
            else if (toggles[1].isOn)
            {
                SaveAndLoad.Save(storedMesh, GenerateBoard.UVTex, "RightEyeSample");
            }
            else if (toggles[2].isOn)
            {
                SaveAndLoad.Save(storedMesh, GenerateBoard.UVTex, "BothEyesSample");
            }
        });

        ui_tool.GetOrAddComponentInChildren<Button>("Read").onClick.AddListener(() =>
        {
            GameObject loadedGrid = GameObject.Find("Grid");

            if (toggles[0].isOn)
            {
                GameObject.Find("Grid").GetComponent<MeshFilter>().mesh = SaveAndLoad.Load("LeftEyeSample");
            }
            else if (toggles[1].isOn)
            {
                GameObject.Find("Grid").GetComponent<MeshFilter>().mesh = SaveAndLoad.Load("RightEyeSample");
            }
            else if (toggles[2].isOn)
            {
                GameObject.Find("Grid").GetComponent<MeshFilter>().mesh = SaveAndLoad.Load("BothEyesSample");
            }

            MoveVertexController.Initilize();

            //if (!GridManager.gridDecoration.Reconstruct(GameObject.Find("Grid").GetComponent<MeshFilter>().mesh, GameObject.Find("Grid").transform))
            //{
            //    Debug.Log("Can't subdivide the grid, the vertices number is same to previous.");
            //}
        });

        ui_tool.GetOrAddComponentInChildren<Button>("Recover").onClick.AddListener(() =>
        {
            GameObject Grid = GameObject.Find("Grid");
            Grid.GetComponent<MeshFilter>().mesh = GridGeneration.Instance().Initilize(GridGeneration.Instance().subdivisionLevel);
            
            GridDecoration.changed = true;
            GridManager.gridDecoration.Update(Grid.GetComponent<MeshFilter>().mesh, Grid.transform);
        });
    }
}