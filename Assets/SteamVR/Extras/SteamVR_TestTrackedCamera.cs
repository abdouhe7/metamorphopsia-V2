//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;

namespace Valve.VR.Extras
{
    public class SteamVR_TestTrackedCamera : MonoBehaviour
    {
        public Material material;
        public static bool undistorted = false;
        public bool cropped = true;
        public int eyes;

        private SteamVR_TrackedCamera.VideoStreamTexture source;
        public static bool isCameraEnabled = true;

#if !UNITY_EDITOR
        private void OnEnable()
        {
            // Acquire the camera stream on enable
            source = SteamVR_TrackedCamera.Source(undistorted);
            source.Acquire();

            // Auto-disable if no camera is present.
            if (!source.hasCamera)
            {
                Debug.Log("No Camera is present");
                // Optionally disable the script here if needed
            }
        }
        private void Start()
        {
            if (source.hasCamera)
            {
                ToggleCameraFeed(false);
            }
        }
        private void OnDestroy()
        {
            // Clear the texture when no longer active.
            material.mainTexture = null;

            // Properly release the camera stream on destroy.
            source.Release();
        }

        private void Update()
        {
            if (!isCameraEnabled) return; // Exit if camera is disabled

            if (!source.hasCamera)
            {
                Debug.Log("No Camera is present. Acquiring one...");
                source.Acquire();
                return;
            }

            Texture2D texture = source.texture;
            if (texture == null)
            {
                Debug.Log("No Source Texture is present");
                return;
            }

            // Apply the latest texture to the material.
            material.mainTexture = texture;

            // Adjust the height of the quad based on the aspect to keep the texels square.
            float aspect = (float)texture.width / texture.height;

            if (cropped)
            {
                if (eyes != 1 && eyes != 0)
                {
                    Debug.Log("Please set target eye.");
                    return;
                }

                material.mainTextureOffset = (eyes == 0 ? new Vector2(0f, 1f) : new Vector2(0f, 0.5f));
                material.mainTextureScale = new Vector2(1f, -0.5f);

                VRTextureBounds_t bounds = source.frameBounds;
                float du = bounds.uMax - bounds.uMin;
                float dv = bounds.vMax - bounds.vMin;

                aspect *= Mathf.Abs(du / dv);
            }
            else
            {
                material.mainTextureOffset = Vector2.zero;
                material.mainTextureScale = new Vector2(1, 1);
            }
        }
#endif

        // New method to toggle the camera feed
        public void ToggleCameraFeed(bool isEnabled)
        {
#if !UNITY_EDITOR
            if (!isEnabled)
            {
                // Release the camera feed to stop it
                source.Release();
                material.mainTexture = null; // Clear texture
                isCameraEnabled = false;
            }
            else
            {
                // Acquire the camera feed to resume it
                source.Acquire();
                isCameraEnabled = true;
            }

            
            Debug.Log("Camera feed " + (isCameraEnabled ? "enabled" : "disabled"));
#endif
        }
        private void OnDisable()
        {
            ToggleCameraFeed(false);
        }
    }
}