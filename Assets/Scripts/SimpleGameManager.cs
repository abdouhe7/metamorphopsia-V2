using CustomGrid;
using System;
using System.Collections;
using System.IO;
using Lessons.L1.Scripts;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Valve.VR.Extras;

public class SimpleGameManager : Singleton<SimpleGameManager>
{
    public int width, height;
    public float offset = 10f; //This offset determines where to place the assessment
    public bool followEyes = false;
    public float gridFollowScale = 0.5f;

    [Range(0f, 10f)] public float eyeFollowResponsiveness = 1f;

    public Transform eyeVectorTransform;
    public GameObject environment;
    public Transform cameraRig;
    public Transform vrCamera;

    public UIController uiController;
    public TMP_Text debugText;
    public GameObject correctionMesh;
    public GameObject realtimeCorrection;
    public Texture imageCorrectionTexture;
    public Texture videoCorrectionTexture;

    public RenderTexture amslerGridTexture;
    public Camera captureCam;

    public GridManager gridManager;
    public AmslerGridEyeTracking amslerGridEyeTracking;
    public CursorGazeBubbleController cursorGazeBubbleController;
    public SmoothPursuitController smoothPursuitController;
    public SaccadeController saccadeController;
    public SteamVR_TestTrackedCamera steamVRCamera;

    public SRAnipalEyeTrackingData eyeTrackingManager;
    public float duration = 60f;
    
    
    
    private bool isCorrectionVisible = false;
    private bool isAmslerGridVisible = false;
    private Texture2D uvMapBoth;
    private Vector3 currentTargetPosition;
    private GazeData currentGazeData;

    private void Awake()
    {
        // Create grid 
        gridManager.ConstructAndDeactivate();
    }

    private void Start()
    {
        StartCoroutine(DebugInformationCoroutine());
        realtimeCorrection.SetActive(false);
        PositionEnvironmentInFront();
    }

    private void PositionEnvironmentInFront()
    {
        environment.transform.parent = cameraRig;

        environment.transform.position = Vector3.zero;

        var tempRotation = environment.transform.rotation;
        tempRotation.x = 0;
        tempRotation.z = 0;
        environment.transform.rotation = tempRotation;
    }

    private IEnumerator DebugInformationCoroutine()
    {
        yield return new WaitForSeconds(5f);
        SaveDebugInformation();
    }

    public void SaveDebugInformation()
    {
        float fps = 1.0f / Time.deltaTime;
        DebugInformation debugInformation = new DebugInformation();
        debugInformation.fps = Mathf.RoundToInt(fps);
        debugInformation.width = Screen.width;
        debugInformation.height = Screen.height;
        debugInformation.gridDistanceFromCamera = gridManager.transform.localPosition.z;
        debugInformation.cameraRigPosition = cameraRig.position;
        debugInformation.cameraRigRotation = cameraRig.rotation.eulerAngles;
        debugInformation.cameraPosition = vrCamera.position;
        debugInformation.cameraRotation = vrCamera.rotation.eulerAngles;
        debugInformation.eyeFollowResponsiveness = eyeFollowResponsiveness;

        SaveAndLoad.Save(debugInformation, "Debug");
    }


    private void Update()
    {
        if (isCorrectionVisible)
        {
            SetUVTexture();
        }

        UpdateAmslerGridPosition();
        GazeDataUpdates();
        ControllerInput();
        DebugUIControls();
    }

    private void ControllerInput()
    {
        if (ControllerOutput.pressMenuButton)
        {
            if (isCorrectionVisible)
            {
                correctionMesh.SetActive(false);
                realtimeCorrection.SetActive(false);
                isCorrectionVisible = false;
                //uiController.SetUIIndex(0);
                //uiController.OpenMenu();
            }
            else if (isAmslerGridVisible)
            {
                gridManager.gameObject.SetActive(false);
                isAmslerGridVisible = false;
                StopEyeTracking();
                //uiController.SetUIIndex(0);
                //uiController.OpenMenu();
            }
        }
    }

    private void DebugUIControls()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            uiController.CloseMenu();
            uiController.SetAssessmentType("AMSLER_GRID");
            uiController.OpenTestsAndCorrections();
        }

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            gridManager.gameObject.SetActive(false);
            isAmslerGridVisible = false;
            uiController.SetUIIndex(5);
            uiController.OpenMenu();
        }
    }

    public void SetGazeData(GazeData gazeData)
    {
        currentGazeData = gazeData;
    }

    public void SetTargetPosition(Vector3 targetPos)
    {
        currentTargetPosition = targetPos;
        eyeTrackingManager.SetTargetPosition(currentTargetPosition);
    }

    private void StartEyeTracking()
    {
        eyeTrackingManager.ClearData();
        eyeTrackingManager.SetDataCollectionEyes(uiController.isLeftEyeSelected, uiController.isRightEyeSelected,
            uiController.isBothEyesSelected);
        eyeTrackingManager.SetDataCollection(true);
    }

    private void StopEyeTracking()
    {
        eyeTrackingManager.SetDataCollection(false);
    }

    private void SaveEyeTracking()
    {
        var fileName = UniqueIdentifierGenerator.GetUniqueIdentifier();
        var assessmentType = uiController.assessmentType.ToString();
        var collectedTargetsCount = cursorGazeBubbleController.GetCollectedTargets();
        SaveAndLoad.Save(assessmentType, eyeTrackingManager.GetData(), collectedTargetsCount, fileName);
    }

    public void Quit()
    {
        Debug.Log("Quit App");
        Application.Quit();
    }

    public void SetDebugText(string text)
    {
        debugText.text = text;
    }

    #region Amsler Grid ET

    public void AmslerGridEyeTracking()
    {
        Debug.Log("AmslerGrid Eye Tracking");
        StartCoroutine(BeginAssessment(duration, amslerGridEyeTracking));
    }

    #endregion

    #region Cursor Gaze Bubble

    public void CursorGazeBubble()
    {
        Debug.Log("Cursor Gaze Bubble");
        StartCoroutine(BeginAssessment(duration, cursorGazeBubbleController));
    }

    private void GazeDataUpdates()
    {
        if (currentGazeData != null && eyeTrackingManager.collectingData)
        {
            cursorGazeBubbleController.UpdateCursorPosition(currentGazeData.bothEyeData);
        }
    }

    public void IncreaseResponsiveness()
    {
        eyeFollowResponsiveness += 0.1f;
    }

    public void DecreaseResponsiveness()
    {
        eyeFollowResponsiveness -= 0.1f;
    }

    public void ToggleFollowEyes()
    {
        followEyes = !followEyes;
    }

    public string GetCursorGazeBubbleInstructions()
    {
        return cursorGazeBubbleController.GetInstructions();
    }

    #endregion

    #region Smooth Pursuit ET

    public void SmoothPursuitEyeTracking()
    {
        Debug.Log("SmoothPursuitEyeTracking");
        StartCoroutine(BeginAssessment(duration, smoothPursuitController));
    }

    public string GetSmoothPursuitInstructions()
    {
        return smoothPursuitController.GetInstructions();
    }

    #endregion

    #region Saccade ET

    public void SaccadeEyeTracking()
    {
        Debug.Log("SaccadeEyeTracking");
        StartCoroutine(BeginAssessment(duration, saccadeController));
    }

    public string GetSaccadeInstructions()
    {
        return saccadeController.GetInstructions();
    }

    #endregion

    #region Generic Assessment

    private IEnumerator BeginAssessment<T>(float duration, T controller) where T : BaseAssessmentController
    {
        uiController.CloseMenu();
        controller.gameObject.SetActive(true);
        controller.Initialize();
        StartEyeTracking();

        yield return new WaitForSeconds(duration);

        StopEyeTracking();
        SaveEyeTracking();
        controller.gameObject.SetActive(false);
        controller.Reset();
        uiController.OpenMenu();
    }

    #endregion

    #region Amsler Grid

    public void UpdateAmslerGridPosition()
    {
        if (followEyes && currentGazeData != null)
        {
            //var eyeTrackingPosition = eyeVectorTransform.localPosition;
            //var gridPosition = gridManager.transform.localPosition;
            //var newPosition = new Vector3(eyeTrackingPosition.x, eyeTrackingPosition.y, offset);
            //gridManager.transform.localPosition = Vector3.Lerp(gridPosition, newPosition, Time.deltaTime * eyeFollowResponsiveness);

            gridManager.transform.position = eyeVectorTransform.position;
            gridManager.transform.rotation = eyeVectorTransform.rotation;
        }
        else
        {
            // Reset grid position relative to camera's forward direction
            gridManager.transform.localPosition = Vector3.forward * offset;
        }
    }

    public string GetAmslerGridInstructions()
    {
        return gridManager.GetInstructions();
    }

    public void Save()
    {
        Debug.Log("Save");
        Mesh storedMesh = gridManager.GetComponent<MeshFilter>().mesh;
        SaveAndLoad.Save(storedMesh, GenerateBoard.UVTex, "Sample");

        CaptureAmslerGrid();
        SaveEyeTracking();
    }

    public void Read()
    {
        Debug.Log("Read");
        gridManager.GetComponent<MeshFilter>().mesh = SaveAndLoad.Load("Sample");
        MoveVertexController.Initilize();
        gridManager.Reconstruct();
    }

    public void Recover()
    {
        Debug.Log("Recover");
        gridManager.GetComponent<MeshFilter>().mesh =
            GridGeneration.Instance().Initilize(GridGeneration.Instance().subdivisionLevel);

        GridDecoration.changed = true;

        GridManager.gridDecoration.Update(gridManager.GetComponent<MeshFilter>().mesh, gridManager.transform);
    }

    public void ResetMesh()
    {
        Debug.Log("ResetMesh");
        gridManager.DestroyGrid();
        gridManager.ConstructAndDeactivate();
        MoveVertexController.Initilize();
    }

    public void AmslerGridInstructions()
    {
        uiController.SetAssessmentType("AMSLER_GRID");
        uiController.SetInstructions();
        uiController.OpenAmslerGridInstuctions();
    }

    public void AmslerGrid()
    {
        GridManager.showTexture = false;
        GridManager.showGrid = true;
        MoveVertexController.showLines = true;
        gridManager.gameObject.SetActive(true);
        isAmslerGridVisible = true;
        StartEyeTracking();
    }

    public void AmslerGridDistorted()
    {
        GridManager.showTexture = true;
        GridManager.showGrid = false;
        MoveVertexController.showLines = false;
        gridManager.gameObject.SetActive(true);
        isAmslerGridVisible = true;
        StartEyeTracking();
    }

    public void Subdivide()
    {
        gridManager.Subdivide();
    }

    public void RealtimeCorrection()
    {
        realtimeCorrection.SetActive(true);
        correctionMesh.SetActive(true);
        isCorrectionVisible = true;
        steamVRCamera.ToggleCameraFeed(true);
        uvMapBoth = SaveAndLoad.ReadUV("Sample");
        correctionMesh.GetComponent<Renderer>().material.mainTexture = videoCorrectionTexture;
    }

    public void ImageCorrection()
    {
        correctionMesh.SetActive(true);
        isCorrectionVisible = true;
        uvMapBoth = SaveAndLoad.ReadUV("Sample");
        correctionMesh.GetComponent<Renderer>().material.mainTexture = imageCorrectionTexture;
    }

    private void SetUVTexture()
    {
        if (uvMapBoth != null)
        {
            correctionMesh.GetComponent<Renderer>().material.SetTexture("_UVTex", uvMapBoth);
            correctionMesh.GetComponent<Renderer>().material.SetFloat("exist", 1.0f);
            SetDebugText("Correction Applied");
        }
        else
        {
            correctionMesh.GetComponent<Renderer>().material.SetFloat("exist", 0.0f);
            SetDebugText("Correction Basic");
        }
    }

    private void CaptureAmslerGrid()
    {
        gridManager.gameObject.SetActive(true);

        captureCam.targetTexture = amslerGridTexture;
        Texture2D screenshot =
            new Texture2D(amslerGridTexture.width, amslerGridTexture.height, TextureFormat.RGB24, false);
        captureCam.Render();

        RenderTexture.active = amslerGridTexture;
        screenshot.ReadPixels(new Rect(0, 0, amslerGridTexture.width, amslerGridTexture.height), 0, 0);
        screenshot.Apply();

        RenderTexture.active = null;
        captureCam.targetTexture = null;

        gridManager.gameObject.SetActive(false);

        var fileName = UniqueIdentifierGenerator.GetUniqueIdentifier();
        SaveAndLoad.Save(screenshot.EncodeToPNG(), fileName);
    }

    #endregion


    public L1GridCreate l1GridCreate;
    public void L1CreateGrid()
    {
        Debug.Log("Lesson 1 amal Grid");
        
        l1GridCreate.lesson1Amal();
    }
}