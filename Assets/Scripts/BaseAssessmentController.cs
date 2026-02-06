using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAssessmentController : MonoBehaviour
{
    public string instructions;

    // Start is called before the first frame update
    public virtual void Initialize()
    {
        SetGridPosition();
    }

    public virtual void Reset()
    {

    }

    public virtual string GetInstructions()
    {
        return instructions;
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
}
