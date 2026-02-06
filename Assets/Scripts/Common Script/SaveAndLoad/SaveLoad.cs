using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using CustomGrid;
using UnityEditor;
using System;

/*
 * Data container classes for serialization
 * Each class holds specific types of game data that needs to be saved/loaded
 */

[System.Serializable]
class MeshInformation
{
    [SerializeField]
    public Vector3[] vertices; // Array of vertex positions for a 3D mesh
    [SerializeField]
    public uint subdivisionLevel; // Level of mesh subdivision (for grid generation)
}

[System.Serializable]
class UVInformation
{
    [SerializeField]
    public Vector2[] uvData; // UV mapping coordinates for textures
}

[System.Serializable]
class UIInformation
{
    [SerializeField]
    public StereoTargetEyeMask currentTarget; // Which eye is currently being targeted in VR
}

[System.Serializable]
class DisplayMode
{
    [SerializeField]
    public bool isIndividual; // Flag for display mode (individual vs other modes)
}

[System.Serializable]
class EyeTrackingData
{
    [SerializeField]
    public string assessmentType; // Type of eye tracking assessment being performed
    [SerializeField]
    public int collectedTargets; // Number of targets collected during assessment
    [SerializeField]
    public List<GazeData> gazeDataList; // List of recorded gaze data points
}

[System.Serializable]
class DebugInformation
{
    [SerializeField]
    public float fps; // Current frames per second
    public int width; // Screen width
    public int height; // Screen height
    public float gridDistanceFromCamera; // Distance between grid and camera
    public Vector3 cameraRigPosition; // Position of VR camera rig
    public Vector3 cameraRigRotation; // Rotation of VR camera rig
    public Vector3 cameraPosition; // Main camera position
    public Vector3 cameraRotation; // Main camera rotation
    public float eyeFollowResponsiveness; // Responsiveness setting for eye tracking
}

/*
 * Static SaveAndLoad utility class
 * Provides centralized file operations for persisting game data
 * Handles serialization of various data types to JSON and binary formats
 */
static class SaveAndLoad
{
    // Static instances of data containers for serialization
    static MeshInformation meshInformation = new MeshInformation();
    static UIInformation uiInformation = new UIInformation();
    static DisplayMode displayMode = new DisplayMode();
    static UVInformation uvInformation = new UVInformation();
    static EyeTrackingData eyeTrackingData = new EyeTrackingData();
    static DebugInformation debugInformation = new DebugInformation();

    /*
     * Converts a RenderTexture to a Texture2D for saving
     * Preserves the active render texture state during conversion
     * @param renderTexture Reference to the RenderTexture to convert
     * @return Converted Texture2D
     */
    static public Texture2D RenderTextureToTexture2D(ref RenderTexture renderTexture)
    {
        // Create new Texture2D with matching dimensions and format
        Texture2D savedTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBAFloat, false);

        // Store current active render texture
        var oldActive = RenderTexture.active;
        RenderTexture.active = renderTexture;

        // Read pixels from render texture to texture2D
        savedTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0, false);
        savedTexture.Apply();

        // Restore previous active render texture
        RenderTexture.active = oldActive;

        return savedTexture;
    }

    /*
     * Saves UV map data from a RenderTexture to disk
     * @param UVmap Reference to the RenderTexture containing UV data
     * @param fileName Base filename to save as (without extension)
     */
    static void SaveUV(ref RenderTexture UVmap, string fileName)
    {
        Texture2D savedTexture = RenderTextureToTexture2D(ref UVmap);
        Texture2DExtension.SaveUncompressed(savedTexture, fileName + "UV", Texture2DExtension.DataFormat.ARGBFloat);
    }

    /*
     * Loads UV map data from disk
     * @param fileName Base filename to load from (without extension)
     * @return Loaded Texture2D or null if file doesn't exist
     */
    static public Texture2D ReadUV(string fileName)
    {
        string saveFileName = Application.dataPath + "/Storage/" + fileName + "UV";
        if (File.Exists(saveFileName))
        {
            // Create texture with dimensions from SimpleGameManager
            Texture2D readTexture = new Texture2D(SimpleGameManager.Instance.width, SimpleGameManager.Instance.height, TextureFormat.RGBAFloat, false);
            Texture2DExtension.ReadUncompressed(readTexture, saveFileName);

            return readTexture;
        }
        return null;
    }

    /*
     * Saves debug information to JSON file
     * @param data DebugInformation object containing debug metrics
     * @param fileName Target filename (without extension)
     */
    static public void Save(DebugInformation data, string fileName)
    {
        // Ensure storage directory exists
        string storagePath = Application.dataPath + "/Storage";
        if (!Directory.Exists(storagePath))
        {
            Directory.CreateDirectory(storagePath);
            Debug.Log("Directory Created: " + storagePath);
        }

        debugInformation = data;

        // Serialize to JSON and write to file
        string saveFileName = storagePath + "/" + fileName;
        string jsonString = JsonUtility.ToJson(debugInformation);
        StreamWriter writer = new StreamWriter(saveFileName + ".json");
        writer.Write(jsonString);
        writer.Close();

        Debug.Log("File Saved: " + saveFileName);
        SimpleGameManager.Instance.SetDebugText("File Saved: " + saveFileName);
    }

    /*
     * Saves eye tracking data to JSON file
     * @param assessmentType Type of assessment performed
     * @param gazeData List of gaze data points
     * @param collectedTargets Number of targets collected
     * @param fileName Target filename (without extension)
     */
    static public void Save(string assessmentType, List<GazeData> gazeData, int collectedTargets, string fileName)
    {
        string storagePath = Application.dataPath + "/Storage";
        if (!Directory.Exists(storagePath))
        {
            Directory.CreateDirectory(storagePath);
            Debug.Log("Directory Created: " + storagePath);
        }

        // Populate eye tracking data structure
        eyeTrackingData.assessmentType = assessmentType;
        eyeTrackingData.gazeDataList = gazeData;
        eyeTrackingData.collectedTargets = collectedTargets;

        // Serialize to JSON and write to file
        string saveFileName = storagePath + "/" + fileName;
        string jsonString = JsonUtility.ToJson(eyeTrackingData);
        StreamWriter writer = new StreamWriter(saveFileName + ".json");
        writer.Write(jsonString);
        writer.Close();

        Debug.Log("File Saved: " + saveFileName);
        SimpleGameManager.Instance.SetDebugText("File Saved: " + saveFileName);
    }

    /*
     * Saves mesh data and associated UV map
     * @param storedMesh Mesh object to save
     * @param UVmap RenderTexture containing UV data
     * @param fileName Target filename (without extension)
     */
    static public void Save(Mesh storedMesh, RenderTexture UVmap, string fileName)
    {
        string storagePath = Application.dataPath + "/Storage";
        if (!Directory.Exists(storagePath))
        {
            Directory.CreateDirectory(storagePath);
            Debug.Log("Directory Created: " + storagePath);
        }

        // Store mesh vertices and subdivision level
        meshInformation.vertices = storedMesh.vertices;
        meshInformation.subdivisionLevel = GridGeneration.Instance().subdivisionLevel;

        // Serialize mesh data to JSON
        string saveFileName = storagePath + "/" + fileName;
        string jsonString = JsonUtility.ToJson(meshInformation);
        StreamWriter writer = new StreamWriter(saveFileName + ".json");
        writer.Write(jsonString);
        writer.Close();

        Debug.Log("File Saved: " + saveFileName);
        SimpleGameManager.Instance.SetDebugText("File Saved: " + saveFileName);
        
        // Save associated UV map
        SaveUV(ref UVmap, saveFileName);
    }

    /*
     * Saves raw byte data (e.g., PNG images) to disk
     * @param data Byte array containing file data
     * @param fileName Target filename (with extension)
     */
    static public void Save(byte[] data, string fileName)
    {
        string storagePath = Application.dataPath + "/Storage";
        if (!Directory.Exists(storagePath))
        {
            Directory.CreateDirectory(storagePath);
            Debug.Log("Directory Created: " + storagePath);
        }
        string saveFileName = storagePath + "/" + fileName + ".png";
        File.WriteAllBytes(saveFileName, data);
        Debug.Log("File Saved: " + saveFileName);
    }

    /*
     * Loads mesh data from JSON file
     * @param fileName Source filename (without extension)
     * @return Loaded Mesh or new default mesh if file doesn't exist
     */
    static public Mesh Load(string fileName)
    {
        string filePath = Application.dataPath + "/Storage/" + fileName + ".json";
        if (File.Exists(filePath))
        {
            // Enable processing component if in Eyes Test scene
            if (SceneManager.GetActiveScene().name == "Eyes Test")
                GameObject.Find("Canvas").GetComponent<Processing>().enabled = true;

            // Read and deserialize JSON data
            StreamReader reader = new StreamReader(filePath);
            string jsonString = reader.ReadToEnd();
            reader.Close();
            meshInformation = JsonUtility.FromJson<MeshInformation>(jsonString);

            // Initialize mesh with saved subdivision level
            Mesh mesh = GridGeneration.Instance().Initilize(meshInformation.subdivisionLevel);
            mesh.SetVertices(meshInformation.vertices);

            SimpleGameManager.Instance.SetDebugText("Mesh Loaded");

            return mesh;
        }

        SimpleGameManager.Instance.SetDebugText("No Load file Found. Creating new!");

        // Fallback to default mesh with subdivision level 1
        return GridGeneration.Instance().Initilize(1);
    }

    /*
     * Saves UI eye target preference
     * @param currentTarget Which eye is currently targeted (left, right, both)
     */
    static public void SaveUI(StereoTargetEyeMask currentTarget)
    {
        uiInformation.currentTarget = currentTarget;
        string jsonString = JsonUtility.ToJson(uiInformation);
        StreamWriter writer = new StreamWriter(Application.dataPath + "/Storage/UI_Information.json");
        writer.Write(jsonString);
        writer.Close();
    }

    /*
     * Loads UI eye target preference
     * @return Saved eye target or None if no preference exists
     */
    static public StereoTargetEyeMask LoadCurrentTargetUI()
    {
        string filePath = Application.dataPath + "/Storage/UI_Information.json";
        if (File.Exists(filePath))
        {
            StreamReader reader = new StreamReader(filePath);
            string jsonString = reader.ReadToEnd();
            reader.Close();
            uiInformation = JsonUtility.FromJson<UIInformation>(jsonString);

            return uiInformation.currentTarget;
        }
        return StereoTargetEyeMask.None;
    }

    /*
     * Saves display mode setting
     * @param mode Boolean indicating display mode (true for individual mode)
     */
    static public void SaveDisplayMode(bool mode)
    {
        displayMode.isIndividual = mode;
        string jsonString = JsonUtility.ToJson(displayMode);
        StreamWriter writer = new StreamWriter(Application.dataPath + "/Storage/DisplayMode.json");
        writer.Write(jsonString);
        writer.Close();
    }

    /*
     * Loads display mode setting
     * @return Saved display mode or false (default) if no setting exists
     */
    static public bool LoadDisplayMode()
    {
        string filePath = Application.dataPath + "/Storage/DisplayMode.json";
        if (File.Exists(filePath))
        {
            StreamReader reader = new StreamReader(filePath);
            string jsonString = reader.ReadToEnd();
            reader.Close();
            displayMode = JsonUtility.FromJson<DisplayMode>(jsonString);

            return displayMode.isIndividual;
        }
        return false;
    }
}