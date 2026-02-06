using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBoard : MonoBehaviour
{
    public static RenderTexture UVTex;
    public bool generateButtons;

    public List<GameObject> buttonPrefabs;

    public float offsetX, offsetY, spacing;

    void Start()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];
        Vector2[] uvCoordinate = new Vector2[4];
        int[] triangles = { 0, 2, 3, 0, 3, 1 };

        float width = (float)SimpleGameManager.Instance.width / 100.0f;
        float height = (float)SimpleGameManager.Instance.height / 100.0f;

        vertices[0] = new Vector3(-width / 2, -height / 2, 0f);
        vertices[1] = new Vector3(width / 2, -height / 2, 0f);
        vertices[2] = new Vector3(-width / 2, height / 2, 0f);
        vertices[3] = new Vector3(width / 2, height / 2, 0f);

        uvCoordinate[0] = new Vector2(0, 0);
        uvCoordinate[1] = new Vector2(1, 0);
        uvCoordinate[2] = new Vector2(0, 1);
        uvCoordinate[3] = new Vector2(1, 1);

        mesh.vertices = vertices;
        mesh.uv = uvCoordinate;
        mesh.triangles = triangles;

        GetComponent<MeshFilter>().mesh = mesh;

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = mesh;
        }

        if (generateButtons)
            CreateSubdivisionButton(width, height);
    }


    // Update is called once per frame
    void Update()
    {
        GetComponent<Renderer>().material.SetTexture("_UVTex", UVTex);

        //if (GridManager.showTexture)
        //    GetComponent<Renderer>().material.SetFloat("_ShowTexture", 1f);
        //else
        GetComponent<Renderer>().material.SetFloat("_ShowTexture", 0f);
    }

    void CreateSubdivisionButton(float width, float height)
    {
        for (int i = 0; i < buttonPrefabs.Count; i++)
        {
            GameObject buttonInstance = Instantiate(buttonPrefabs[i], transform);
            buttonInstance.transform.localPosition = new Vector3(offsetX + (spacing * i) + -width / 2, offsetY + -height / 2, 0f);
        }
    }

}
