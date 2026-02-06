using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UIController : MonoBehaviour
{
    [SerializeField] GameObject[] menus;
    public AssessmentType assessmentType;
    public TMP_Text instructionsText;

    public GameObject amslerGridInstructionsPanel;


    // We need to save information about the settings applied in each panel
    // For example that would be simply, Both Eyes, Left Eye and Right Eye
    public bool isBothEyesSelected;
    public bool isLeftEyeSelected;
    public bool isRightEyeSelected;

    private int lastActiveUIIndex = 0;

    // We will need to decide which menu to open here
    public void OpenMenu()
    {
        // Close All Menus
        CloseMenu();
        // Open Menu
        menus[lastActiveUIIndex].SetActive(true);
    }

    public void CloseMenu()
    {
        // Just close all menus
        foreach (var menu in menus)
        {
            menu.SetActive(false);
        }
        //SetAssessmentType("NONE");
    }

    public void OpenAmslerGridInstuctions()
    {
        amslerGridInstructionsPanel.SetActive(true);
    }

    public void SetUIIndex(int index)
    {
        Debug.Log(index);
        lastActiveUIIndex = index;
    }

    public void SetAssessmentType(string type)
    {
        assessmentType = (AssessmentType)System.Enum.Parse(typeof(AssessmentType), type);
    }

    public void SetInstructions()
    {
        string instructions = "";
        // Decide which scene to open when Next is pressed
        switch (assessmentType)
        {
            case AssessmentType.SMOOTH_PURSUIT:
                instructions = SimpleGameManager.Instance.GetSmoothPursuitInstructions();
                break;
            case AssessmentType.SACCADE:
                instructions = SimpleGameManager.Instance.GetSaccadeInstructions();
                break;
            case AssessmentType.CURSOR_GAZE_BUBBLE:
                instructions = SimpleGameManager.Instance.GetCursorGazeBubbleInstructions();
                break;
            case AssessmentType.AMSLER_GRID:
                instructions = SimpleGameManager.Instance.GetAmslerGridInstructions();
                break;
            case AssessmentType.NONE:
                instructions = "";
                break;
        }
        instructionsText.text = instructions;
    }


    
    public void OpenTestsAndCorrections()
    {
        // Decide which scene to open when Next is pressed
        switch (assessmentType)
        {
            case AssessmentType.SMOOTH_PURSUIT:
                SimpleGameManager.Instance.SmoothPursuitEyeTracking();
                break;
            case AssessmentType.SACCADE:
                SimpleGameManager.Instance.SaccadeEyeTracking();
                break;
            case AssessmentType.AMSLER_GRID_ET:
                SimpleGameManager.Instance.AmslerGridEyeTracking();
                break;
            case AssessmentType.AMSLER_GRID:
                SimpleGameManager.Instance.AmslerGrid();
                break;
            case AssessmentType.CURSOR_GAZE_BUBBLE:
                SimpleGameManager.Instance.CursorGazeBubble();
                break;
            case AssessmentType.NONE:
                break;
        }
    }

    public void SetBothEyes()
    {
        isBothEyesSelected = !isBothEyesSelected;
    }

    public void SetLeftEye()
    {
        isLeftEyeSelected = !isLeftEyeSelected;
    }

    public void SetRightEye()
    {
        isRightEyeSelected = !isRightEyeSelected;
    }

    public void ResetSelections()
    {
        isBothEyesSelected = false;
        isLeftEyeSelected = false;
        isRightEyeSelected = false;
    }

}
