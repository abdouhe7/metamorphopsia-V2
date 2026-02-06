using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMesh : MonoBehaviour
{
    static public Mesh DisplayMesh()
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

        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvCoordinate);
        mesh.SetTriangles(triangles, 0);

        return mesh;
    }

    void Start()
    {
        GetComponent<MeshFilter>().mesh = DisplayMesh();
    }
}
