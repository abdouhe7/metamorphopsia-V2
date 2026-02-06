using UnityEngine;

namespace CustomGrid
{
    public partial class GridDecoration
    {
        private void SetAttributes(ref Mesh mesh, ref Transform transform_)
        {
            triangles = mesh.triangles;
            vertices = mesh.vertices;
            transform = transform_;
        }

        public GridDecoration(Mesh mesh, Transform transform_)
        {
            SetAttributes(ref mesh, ref transform_);

            LoadPointModel();
            InitilizePoints();
        }

        public void Update(Mesh mesh, Transform transform_)
        {
            SetAttributes(ref mesh, ref transform_);

            if (changed)
            {
                UpdatePoints();
                //UpdateLines();
                changed = false;
            }
        }

        public bool Reconstruct(Mesh mesh, Transform transform_)
        {
            SetAttributes(ref mesh, ref transform_);

            ReconstructPoints();
            //ReconstructLines();
            UpdatePoints();

            return true;
        }

        //attribute
        private int[] triangles;
        private Vector3[] vertices;
        private Transform transform;

        public static bool changed;
    }
}

