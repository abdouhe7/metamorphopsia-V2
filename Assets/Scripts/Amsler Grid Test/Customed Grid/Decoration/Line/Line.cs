using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
    private float scale;
    private Vector3[] points;

    private GameObject lineObject;
    private Mesh lineMesh;

    private Material lineMaterial;

    public Line(Vector3[] points_, Material lineMaterial_ = null, float sacle_ = 0.03f, string lineName = null)
    {
        lineObject = new GameObject();
        lineObject.name = lineName == null ? "Default Line" : lineName;

        this.lineMaterial = lineMaterial_;
        if (lineMaterial == null)
            lineMaterial = new Material(Shader.Find("Standard"));

        points = new Vector3[points_.Length];
        for (int i = 0; i < points.Length; ++i)
            points[i] = points_[i];

        this.scale = sacle_;

        lineMesh = new Mesh();
        GenerateLine();
        GenerateLineTriangle();

        lineObject.AddComponent<MeshFilter>().mesh = lineMesh;
        lineObject.AddComponent<MeshRenderer>();
        lineObject.GetComponent<Renderer>().material = lineMaterial;
        lineObject.layer = 7;
    }

    public void SetTransfomParent(Transform transform)
    {
        lineObject.transform.SetParent(transform);
    }

    public void SetPoint(int index, Vector3 value)
    {
        points[index] = value;
        GenerateLine();
    }

    public bool ResetPoints(Vector3[] vertices)
    {
        for (int i = 0; i < points.Length; ++i)
        {
            if (points[i] != vertices[i])
            {
                SetPoint(i, vertices[i]);

                return true;
            }
        }
        return false;
    }

    void GenerateLine()
    {
        Vector3[] vectors = new Vector3[points.Length-1];
        Vector3[] normals = new Vector3[points.Length];

        for (int i = 0; i < points.Length - 1; ++i)
        {
            vectors[i] = points[i + 1] - points[i];
            
            if (i == 0)
                normals[i] = new Vector3(-vectors[i].y, vectors[i].x, 0f).normalized;

            else
            {
                normals[i] = ((new Vector3(-vectors[i].y, vectors[i].x, 0f).normalized + normals[i - 1])).normalized;
                float cosineTheta = Mathf.Clamp(Vector3.Dot(normals[i - 1], normals[i]), 0.70710678118f,1);
                normals[i] /= cosineTheta;
            }
        }

        normals[normals.Length - 1] = 
        new Vector3(-vectors[vectors.Length-1].y, vectors[vectors.Length - 1].x, 0f).normalized;

        CalculatePointsPosition(normals);
    }

    void CalculatePointsPosition(Vector3[] normals)
    {
        Vector3[] generatedPoints = new Vector3[points.Length * 2];
        int index = 0;

        for (int i = 0; i < points.Length; i++)
        {
            generatedPoints[index++] = points[i] - (normals[i] * scale);
            generatedPoints[index++] = points[i] + (normals[i] * scale);
        }

        lineMesh.SetVertices(generatedPoints);
    }

    void GenerateLineTriangle()
    {
        int[] triangles = new int[(points.Length - 1) * 6];

        int index = 0;
        for (int i = 0; i < points.Length - 1; i++)
        {
            triangles[index++] = i * 2;
            triangles[index++] = i * 2 + 1;
            triangles[index++] = i * 2 + 2;
        }
        for (int i = 0; i < points.Length - 1; i++)
        {
            triangles[index++] = i * 2 + 1;
            triangles[index++] = i * 2 + 3;
            triangles[index++] = i * 2 + 2;
        }

        lineMesh.triangles = triangles;
    }
}
