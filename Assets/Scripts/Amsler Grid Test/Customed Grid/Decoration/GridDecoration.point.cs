using UnityEngine;

namespace CustomGrid
{
    public partial class GridDecoration
    {
        GameObject pointDecoration;
        GameObject pointImage;
        GameObject[] points;
        float scale = 0.1f;

        private void LoadPointModel()
        {
            const string pointPath = "Model/Point";
            pointImage = Resources.Load(pointPath) as GameObject;

            pointDecoration = new GameObject("Points");
            pointDecoration.transform.SetParent(transform);
            pointDecoration.transform.localPosition = transform.position;

            if (pointImage == null)
            {
                Debug.Log("Loading point image fault.");
            }
        }
        private void InitilizePoints()
        {
            points = new GameObject[vertices.Length];
            Vector3 pointScale = Vector3.one * (scale - (0.02f * (GridGeneration.Instance().subdivisionLevel - 1)));

            for (int i = 0; i < vertices.Length; ++i)
            { 
                points[i] = GameObject.Instantiate(pointImage);

                points[i].transform.SetParent(pointDecoration.transform);
                points[i].transform.localScale = pointScale;
                points[i].transform.position = transform.localToWorldMatrix.MultiplyPoint(vertices[i]);
                var pointData = points[i].GetComponent<PointData>();
                pointData.index = i;
            }

            points[points.Length / 2].transform.localScale *= 2;
        }

        private void ReconstructPoints()
        {
            DestroyPoints();
            InitilizePoints();
            //MoveVertexController.ShowDotsUI.Invoke();
        }

        void UpdatePoints()
        {
            for (int i = 0; i < points.Length; ++i)
            {
                points[i].transform.position = transform.localToWorldMatrix.MultiplyPoint(vertices[i]);
            }
        }

        public void DestroyPoints()
        {
            int childCount = pointDecoration.transform.childCount;
            if (childCount > 0)
            {
                for (int i = 0; i < childCount; ++i)
                {
                    GameObject.Destroy(pointDecoration.transform.GetChild(i).gameObject);
                }
            }
            if (pointDecoration.transform.childCount == 0)
                GameObject.Destroy(pointDecoration);
        }

        public void DestroyParent()
        {
            GameObject.Destroy(pointDecoration);
        }

        public GameObject[] GetPoints()
        {
            return points;
        }

    }
}
