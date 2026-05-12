using System.Collections;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class SaveMeshDataAndScreenshot : MonoBehaviour
{
    [Header("Output")]
    public string folderName = "SavedWarpData";
    public string fileName = "patient_01";

    [Header("Grid Dimensions")]
    public int cols = 19;
    public int rows = 11;

    private MeshFilter meshFilter;
    private Vector3[] originalVertices;

    [System.Serializable]
    public class MeshData
    {
        public int cols;
        public int rows;
        public Vector3[] originalVertices;
        public Vector3[] currentVertices;
        public Vector3[] displacement;
        public Vector2[] uv;
    }

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();

        if (meshFilter != null && meshFilter.mesh != null)
        {
            originalVertices = meshFilter.mesh.vertices;
        }
    }

    [ContextMenu("Save Data and Screenshot")]
    public void SaveDataAndScreenshot()
    {
        SaveMeshAsJson();
        StartCoroutine(SaveScreenshotCoroutine());
    }

    public void SaveMeshAsJson()
    {
        Mesh mesh = GetCurrentMesh();
        if (mesh == null) return;

        Vector3[] current = mesh.vertices;
        Vector2[] uv = mesh.uv;

        if (originalVertices == null || originalVertices.Length != current.Length)
        {
            originalVertices = new Vector3[current.Length];

            for (int i = 0; i < current.Length; i++)
                originalVertices[i] = current[i];
        }

        Vector3[] displacement = new Vector3[current.Length];

        for (int i = 0; i < current.Length; i++)
        {
            displacement[i] = current[i] - originalVertices[i];
        }

        MeshData data = new MeshData
        {
            cols = cols,
            rows = rows,
            originalVertices = originalVertices,
            currentVertices = current,
            displacement = displacement,
            uv = uv
        };

        string folder = Application.dataPath + "/" + folderName;

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string path = folder + "/" + fileName + "_data.json";

        File.WriteAllText(path, JsonUtility.ToJson(data, true));

        Debug.Log("JSON saved: " + path);
    }

    IEnumerator SaveScreenshotCoroutine()
    {
        yield return new WaitForEndOfFrame();

        string folder = Application.dataPath + "/" + folderName;

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string path = folder + "/" + fileName + "_image.png";

        ScreenCapture.CaptureScreenshot(path);

        Debug.Log("Screenshot saved: " + path);
    }

    Mesh GetCurrentMesh()
    {
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();

        if (meshFilter == null || meshFilter.mesh == null)
        {
            Debug.LogWarning("No mesh found on MeshFilter.");
            return null;
        }

        return meshFilter.mesh;
    }
}