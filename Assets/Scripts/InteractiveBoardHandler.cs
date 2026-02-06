using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractiveBoardHandler : MonoBehaviour
{
    public AssessmentType assessmentType;
    public TMP_Text labelText;

    private AudioSource audioSource;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        ChoosePointWithRaycast();

        mouseRay();
    }

    private void mouseRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
        if (Physics.Raycast(ray, out hit))
        {
            var interactiveBoardHandler = hit.collider.GetComponent<InteractiveBoardHandler>();

            if (interactiveBoardHandler != null && interactiveBoardHandler.Equals(this))
            {
                if (Input.GetMouseButtonUp(0))
                {
                    BoardPressed();
                }
            }
        }
    }

    void ChoosePointWithRaycast()
    {
        var pointObject = ControllerOutput.hitObjectRight;

        if (pointObject != null)
        {
            var interactiveBoardHandler = pointObject.GetComponent<InteractiveBoardHandler>();
            if (interactiveBoardHandler != null && interactiveBoardHandler.Equals(this))
            {
                if (ControllerOutput.pressUpPrimaryButton)
                {
                    BoardPressed();
                }
            }
        }
    }

    void BoardPressed()
    {
        audioSource.Play();
        Debug.Log(assessmentType.ToString());
        switch (assessmentType)
        {
            case AssessmentType.AMSLER_GRID:
                SimpleGameManager.Instance.AmslerGridInstructions();
                break;
            case AssessmentType.AMSLER_GRID_DISTORED:
                SimpleGameManager.Instance.AmslerGridDistorted();
                break;
            case AssessmentType.REALTIME_CORRECTION:
                SimpleGameManager.Instance.RealtimeCorrection();
                break;
            case AssessmentType.IMAGE_CORRECTION:
                SimpleGameManager.Instance.ImageCorrection();
                break;
            case AssessmentType.L1Amal:
                SimpleGameManager.Instance.L1CreateGrid();
                break;
            default:
                SimpleGameManager.Instance.Quit();
                break;
        }
    }
}