using UnityEngine;
using ViveSR.anipal.Eye; // Importing the namespace for the Vive Eye Tracker SDK.

[System.Serializable]
public class GazeData
{
    [SerializeField]
    public float timestamp;

    [SerializeField]
    public Vector3 leftEyeData;
    [SerializeField]
    public Vector3 leftEyeRotation;
    [SerializeField]
    public float leftEyeOpenness;

    [SerializeField]
    public Vector3 rightEyeData;
    [SerializeField]
    public Vector3 rightEyeRotation;
    [SerializeField]
    public float rightEyeOpenness;

    [SerializeField]
    public Vector3 bothEyeData;
    [SerializeField]
    public Vector3 bothEyeRotation;

    [SerializeField]
    public Vector3 targetPosition;

    public GazeData(float timestamp, Vector3 leftData, Vector3 leftEyeRotation, float leftOpenness, Vector3 rightData, Vector3 rightEyeRotation, float rightOpenness, Vector3 bothData, Vector3 bothEyeRotation, Vector3 targetPosition)
    {
        this.timestamp = timestamp;
        this.leftEyeData = leftData;
        this.leftEyeRotation = leftEyeRotation;
        this.leftEyeOpenness = leftOpenness;
        this.rightEyeData = rightData;
        this.rightEyeRotation = rightEyeRotation;
        this.rightEyeOpenness = rightOpenness;
        this.bothEyeData = bothData;
        this.bothEyeRotation = bothEyeRotation;
        this.targetPosition = targetPosition;
    }
}