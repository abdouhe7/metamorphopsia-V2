using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class CameraFeedCheck : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        SteamVR_TrackedCamera.VideoStreamTexture source = SteamVR_TrackedCamera.Source(false);

        if (!source.hasCamera)
        {
            Debug.Log("No Camera is present. Acquiring one...");
            GetComponent<Renderer>().material.color = Color.red;
            source.Acquire();
            return;
        }

        GetComponent<Renderer>().material.color = Color.green;
    }
}
