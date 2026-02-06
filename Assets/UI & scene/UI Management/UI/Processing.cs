using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Processing : MonoBehaviour
{
    Vector3 angle = Vector3.zero;
    float counter = 0f;

    IEnumerator Process()
    {
        while (counter < 1f)
        {
            counter += Time.deltaTime;
            EyeTestScene.panelManager.GetPeek().ui_tool.Rotate(angle);
            angle.z += 2f;

            yield return null;
        }
        this.enabled = false;
    }

    private void OnEnable()
    {
        EyeTestScene.panelManager.Push(new ProcessPanel());
        StartCoroutine(Process());
    }

    private void OnDisable()
    {
        StopCoroutine(Process());
        EyeTestScene.panelManager.Pop();
        counter = 0f;
        angle = Vector3.zero;
    }
}
