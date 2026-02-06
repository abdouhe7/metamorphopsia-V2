using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaccadeController : BaseAssessmentController
{
    public Transform target;
    public bool horizontalSaccade = false;
    public bool verticalSaccade = false;
    public float radius;

    private float currentPosition = 0f;

    public override void Reset()
    {
        base.Reset();
        currentPosition = 0f;
        target.localPosition = Vector3.zero;
    }

    public override void Initialize()
    {
        base.Initialize();
        SaccadePositionChange();
        StartCoroutine(TimeLapsedCheckerSaccade());
    }

    public IEnumerator TimeLapsedCheckerSaccade()
    {
        while(true)
        {
            Debug.Log("One second has passed");
            SaccadePositionChange();
            yield return new WaitForSeconds(1f);
        }
    }

    void SaccadePositionChange()
    {
        float angle = (horizontalSaccade || verticalSaccade) ? currentPosition : Mathf.PI; // 0 for up, PI for down
        float x = Mathf.Cos(angle) * radius; // Use Cos for vertical
        float y = Mathf.Cos(angle) * radius; // Use Cos for vertical
                                             // 
        if (currentPosition == 0)
            currentPosition = Mathf.PI;
        else
            currentPosition = 0;

        if (horizontalSaccade)
        {
            // Update the position of the object
            target.localPosition = Vector3.right * x;
        }
        else if (verticalSaccade)
        {
            // Update the position of the object
            target.localPosition = Vector3.up * y;
        }

        SimpleGameManager.Instance.SetTargetPosition(target.localPosition);
    }
}
