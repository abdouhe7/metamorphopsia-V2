using UnityEngine;

namespace CustomGrid
{
    public sealed partial class GridGeneration
    {
        Vector3[] originalVertices;
        Vector3[] newVertices;

        public Mesh Subdivision(Mesh mesh)
        {
            if (++subdivisionLevel > maxSubdivision)
            {
                subdivisionLevel = (uint)maxSubdivision;
                Debug.Log("The mesh can not be divided anymore.");
                return null;
            }
            MoveVertexController.ResetSelectedPointPosition.Invoke();

            originalVertices = mesh.vertices;

            int originalWidth = verticesWidthNumber;
            int originalHeight = verticesHeightNumber;

            int newWidth = 2 * originalWidth - 1;
            int newHeight = 2 * originalHeight - 1;

            newVertices = new Vector3[newWidth * newHeight];

            CopyOriginalPosition(newWidth, newHeight, originalWidth);
            CalculateEdgePointsPosition(newWidth, newHeight);
            CalculateCentrePointPosition(newWidth, newHeight);

            //build new mesh
            verticesHeightNumber = newHeight;
            verticesWidthNumber = newWidth;
            verticesNumber = newVertices.Length;

            SetVertices(verticesData: newVertices);
            SetTriangles();

            return meshSample;
        }

        //Set the basic meshtopology as same as the previous mesh
        private void CopyOriginalPosition(int newWidth, int newHeight, int originalWidth)
        {
            for (int yIndex = 0, yIndexOrigin = 0; yIndex < newHeight; yIndex += 2, yIndexOrigin++)
            {
                for (int xIndex = 0, xIndexOrigin = 0; xIndex < newWidth; xIndex += 2, xIndexOrigin++)
                {
                    int index = xIndex + yIndex * newWidth;
                    int indexOrigin = xIndexOrigin + yIndexOrigin * originalWidth;

                    newVertices[index] = originalVertices[indexOrigin];
                }
            }
        }

        //Calculate the edge points' position of every blcok
        private void CalculateEdgePointsPosition(int newWidth, int newHeight)
        {
            for (int yIndex = 0; yIndex < newHeight; yIndex++)
            {
                if (yIndex % 2 == 0)
                {
                    for (int xIndex = 1; xIndex < newWidth - 1; xIndex += 2)
                    {
                        int index = xIndex + yIndex * newWidth;

                        newVertices[index] = (newVertices[index - 1] + newVertices[index + 1]) / 2;
                    }
                }
                else
                {
                    for (int xIndex = 0; xIndex < newWidth; xIndex += 2)
                    {
                        int index = xIndex + yIndex * newWidth;

                        newVertices[index] = (newVertices[index - newWidth] + newVertices[index + newWidth]) / 2;
                    }
                }
            }

        }

        //Calculate the centre point's position of every block
        private void CalculateCentrePointPosition(int newWidth, int newHeight)
        {
            for (int yIndex = 1; yIndex < newHeight - 1; yIndex += 2)
            {
                for (int xIndex = 1; xIndex < newWidth - 1; xIndex += 2)
                {
                    int index = xIndex + yIndex * newWidth;
                    Vector3 AB = newVertices[index + 1] - newVertices[index - 1];
                    Vector3 CD = newVertices[index + newWidth] - newVertices[index - newWidth];

                    float t = Vector3.Cross((newVertices[index - newWidth] - newVertices[index - 1]), AB).magnitude
                        / Vector3.Cross(CD, AB).magnitude;

                    newVertices[index] = newVertices[index - newWidth] + CD * t;
                }
            }
        }
    }
}
