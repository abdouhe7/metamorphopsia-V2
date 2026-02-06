using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinocularDebugger : MonoBehaviour
{
    public bool isLeftEye = false;
    public bool isRightEye = false;
    public bool isBothEye = false;

    public void SampleUpdate(GazeData gazeData)
    {
        if (isBothEye)
            transform.localPosition = new Vector3(gazeData.bothEyeData.x, gazeData.bothEyeData.y, gazeData.bothEyeData.z);
        else if (isLeftEye)
            transform.localPosition = new Vector3(gazeData.leftEyeData.x, gazeData.leftEyeData.y, gazeData.leftEyeData.z);
        else if (isRightEye)
            transform.localPosition = new Vector3(gazeData.rightEyeData.x, gazeData.rightEyeData.y, gazeData.rightEyeData.z);
    }

    public void SampleUpdateAdvanced(GazeData gazeData)
    {
        if (isBothEye)
        {
            transform.localPosition = gazeData.bothEyeData;
            transform.eulerAngles = gazeData.bothEyeRotation;
        }
        else if (isLeftEye)
        {
            transform.localPosition = gazeData.leftEyeData;
            transform.eulerAngles = gazeData.leftEyeRotation;
        }
        else if (isRightEye)
        {
            transform.localPosition = gazeData.rightEyeData;
            transform.eulerAngles = gazeData.rightEyeRotation;
        }
    }
}
