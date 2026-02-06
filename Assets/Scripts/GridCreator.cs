using UnityEngine;

public class GridCreator : MonoBehaviour
{
    public GameObject gridPointPrefab; // Assign a prefab for grid points in the inspector
    public int width;
    public int height;
    public float spacing;

    private void Start()
    {
        CreateGrid(width, height, spacing);
    }

    public void CreateGrid(int width, int height, float spacing)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * spacing, y * spacing, 0);
                GameObject gridPoint = Instantiate(gridPointPrefab, position, Quaternion.identity, transform);
                gridPoint.name = $"GridPoint_{x}_{y}";
            }
        }
    }
}
