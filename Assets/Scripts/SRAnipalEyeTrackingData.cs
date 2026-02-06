using UnityEngine;
using ViveSR.anipal.Eye; // Importing the namespace for the Vive Eye Tracker SDK.
using System.Text.RegularExpressions; // For using regular expressions.
using System.Collections; // For using coroutines.
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;

public class SRAnipalEyeTrackingData : MonoBehaviour // Defining a new class EyeGazeRaycast, inheriting from MonoBehaviour.
{
    [Serializable]
    public class EyeSampleEvent : UnityEvent<GazeData>
    {

    }
    [SerializeField]
    protected EyeSampleEvent _eyeSampleUpdated;

    public bool collectingData = false; // Flag to control data collection
    //public bool useDebugVector;
    //public Transform eyeVectorTransform;
    public Camera mainCamera;

    // List to store gaze data
    private List<GazeData> gazeDataList = new List<GazeData>();
    private Vector3 targetPosition;
    private float elapsedTime = 0f;
    private bool collectLeftEyeData, collectRightEyeData, collectBothEyeData;
    private Vector3 binocularVector, leftEyeVector, rightEyeVector = Vector3.zero;
    private Vector3 binocularRotation, leftEyeRotation, rightEyeRotation = Vector3.zero;
    private float renderDistance;

    private void Start()
    {
        if (!SRanipal_Eye_Framework.Instance.EnableEye)
        {
            enabled = false;
            return;
        }
        //if (!useDebugVector) eyeVectorTransform.gameObject.SetActive(false);
        //SetRenderDistance();
        renderDistance = SimpleGameManager.Instance.offset;
    }

    void SetRenderDistance()
    {
        var width = SimpleGameManager.Instance.width / 100.0f;
        var height = SimpleGameManager.Instance.height / 100.0f;
        var screenRatio = height / width;

        var gridOffset = SimpleGameManager.Instance.offset;
        renderDistance = gridOffset + screenRatio * gridOffset;
    }

    public void SetTargetPosition(Vector3 targetPos)
    {
        targetPosition = targetPos;
    }

    private void Update() // Update is called once per frame.
    {
        if(collectingData)
        {
            elapsedTime += Time.deltaTime;

            float leftEyeOpenness = 0f; 
            float rightEyeOpenness = 0f;

            if (collectLeftEyeData && SRanipal_Eye_v2.GetEyeOpenness(EyeIndex.LEFT, out leftEyeOpenness))
            {
                Debug.Log("leftEyeOpenness: " + leftEyeOpenness);
            }

            if (collectRightEyeData && SRanipal_Eye_v2.GetEyeOpenness(EyeIndex.RIGHT, out rightEyeOpenness))
            {
                Debug.Log("rightEyeOpenness: " + rightEyeOpenness);
            }

#if !UNITY_EDITOR
            BinocularEyeTracking();

            LeftEyeTracking();

            RightEyeTracking();
#else
            MouseTracking();
#endif
            //if(useDebugVector)
            //{
            //    eyeVectorTransform.position = GetEyeDataInWorldSpace();
            //}

            var gazeData = new GazeData(elapsedTime, leftEyeVector, leftEyeRotation, leftEyeOpenness, rightEyeVector, rightEyeRotation, rightEyeOpenness, binocularVector, binocularRotation, targetPosition);
            _eyeSampleUpdated?.Invoke(gazeData);

            gazeDataList.Add(gazeData);

            SimpleGameManager.Instance.SetGazeData(gazeData);

        }
    }

    private void BinocularEyeTracking()
    {
        // Obtaining the combined gaze direction for both eyes and storing it in gazeRay.
        if (collectBothEyeData && SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out var gazeRay))
        {
            // Binocular
            var origin = gazeRay.origin; //eyePosition
            var dir = gazeRay.direction; //eyeRotation
            //var depth = renderDistance - origin.z;
            //var eyePosition = origin + depth * (dir / dir.z);
            //binocularVector = new Vector3(eyePosition.x, eyePosition.y, renderDistance);

            binocularVector = origin + dir * renderDistance;
            binocularRotation = Quaternion.LookRotation(dir, Vector3.up).eulerAngles;
        }
    }

    private void LeftEyeTracking()
    {
        if (collectLeftEyeData && SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out var leftGazeRay))
        {
            // Left
            var origin = leftGazeRay.origin; //eyePosition
            var dir = leftGazeRay.direction; //eyeRotation
            //var depth = renderDistance - origin.z;
            //var eyePosition = origin + depth * (dir / dir.z);
            //leftEyeVector = new Vector3(eyePosition.x, eyePosition.y, renderDistance);
            leftEyeVector = origin + dir * renderDistance;
            leftEyeRotation = Quaternion.LookRotation(dir, Vector3.up).eulerAngles;

        }
    }

    private void RightEyeTracking()
    {
        if (collectRightEyeData && SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out var rightGazeRay))
        {
            // Right
            var origin = rightGazeRay.origin; //eyePosition
            var dir = rightGazeRay.direction; //eyeRotation
            //var depth = renderDistance - origin.z;
            //var eyePosition = origin + depth * (dir / dir.z);
            //rightEyeVector = new Vector3(eyePosition.x, eyePosition.y, renderDistance);
            rightEyeVector = origin + dir * renderDistance;
            rightEyeRotation = Quaternion.LookRotation(dir, Vector3.up).eulerAngles;
        }
    }

    private void MouseTracking()
    {
        var ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        var origin = new Vector3(ray.origin.x, ray.origin.x, ray.origin.x);
        var dir = new Vector3(ray.direction.x, ray.direction.y, ray.direction.z);
        var depth = renderDistance - origin.z;
        Debug.DrawRay(origin, dir * renderDistance, Color.red);
        
        // Set transform at ray point in circle
        var eyePosition = origin + dir * renderDistance;
        var eyeRotation = Quaternion.LookRotation(dir, Vector3.up).eulerAngles;

        binocularVector = leftEyeVector = rightEyeVector = eyePosition;
        binocularRotation = leftEyeRotation = rightEyeRotation = eyeRotation;

        // Set transform at ray point on a plane
        //var eyePosition = origin + depth * (dir / dir.z);
        //binocularVector = leftEyeVector = rightEyeVector = eyePosition;
    }

    public List<GazeData> GetData()
    {
        if(gazeDataList != null)
            return gazeDataList;
        return new List<GazeData>();
    }

    public void ClearData()
    {
        gazeDataList.Clear();
        elapsedTime = 0f;
        collectLeftEyeData = collectRightEyeData = collectBothEyeData = false;
        binocularVector = leftEyeVector = rightEyeVector = Vector3.zero;
        SimpleGameManager.Instance.SetGazeData(null);
    }

    public void SetDataCollectionEyes(bool left, bool right, bool both)
    {
        //collectLeftEyeData = left;
        //collectRightEyeData = right;
        //collectBothEyeData = both;
        collectBothEyeData = collectLeftEyeData = collectRightEyeData = true;
    }

    public void SetDataCollection(bool state)
    {
        collectingData = state;
        //if (useDebugVector)
        //    eyeVectorTransform.gameObject.SetActive(state);
    }

    public Vector3 GetEyeDataInWorldSpace()
    {
        // Convert binocularVector (world-space gaze position) to local space relative to the camera
        Vector3 localGazePosition = Camera.main.transform.InverseTransformPoint(binocularVector);

        // Project local position onto a plane at renderDistance in front of the camera
        Vector3 screenSpacePosition = new Vector3(
            localGazePosition.x,
            localGazePosition.y,
            renderDistance
        );

        // Convert back to world space at the given render distance
        Vector3 worldPosition = Camera.main.transform.TransformPoint(screenSpacePosition);

        return worldPosition;
    }

    //public Vector3 GetEyeDataInScreenSpace()
    //{
    //    // Calculate the direction from the camera to the binocular vector (eye gaze position in world space)
    //    Vector3 direction = (binocularVector - Camera.main.transform.position).normalized;

    //    // Project the gaze onto a plane at 'renderDistance' in front of the camera
    //    Vector3 projectedPosition = Camera.main.transform.position + direction * renderDistance;

    //    // Return the projected world position
    //    return projectedPosition;
    //}



}