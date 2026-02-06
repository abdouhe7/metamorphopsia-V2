using UnityEngine;

namespace CustomGrid
{
    public sealed partial class GridGeneration
    {
        /*singleton mode*/
        private static readonly GridGeneration instance = new GridGeneration();
        static GridGeneration() { }
        private GridGeneration() { }
        public static GridGeneration Instance() { return instance; }

        /*mesh information*/
        public uint subdivisionLevel { get; set; }
        public int GetWidthVerticesNumber() { return verticesWidthNumber; }
        public int GetHeightVerticesNumber() { return verticesHeightNumber; }


        int verticesWidthNumber = 0;
        int verticesHeightNumber = 0;
        int verticesNumber = 0;

        float width = 0;
        float height = 0;
        float screenRatio;

        Mesh meshSample;

        public int maxSubdivision = 4;

        public bool AbleSubdivde()
        {
            return subdivisionLevel != maxSubdivision;
        }
    }
}