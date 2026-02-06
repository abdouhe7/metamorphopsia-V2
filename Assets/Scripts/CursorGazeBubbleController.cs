using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CursorGazeBubbleController : BaseAssessmentController
{
    public Transform cursorTransform;
    public GameObject targetPrefab; // Prefab of the 3D sphere
    public Vector2 minLimit, maxLimit;
    public float maxTargetRadius, minTargetRadius;
    public float interactableDistance;
    public List<Transform> targetPrefabList = new List<Transform>(); // List of dummy spheres
    public Transform targetsParentTransform;
    public int count = 20;
    public float scaleLerpSpeed = 1f;
    public int collectedTargets = 0;
    public AudioSource audioSource;

    public override void Reset()
    {
        DestroyAllTargets();
        cursorTransform.localPosition = Vector3.zero;
        cursorTransform.localScale = Vector3.one;
        collectedTargets = 0;
    }

    private void OnEnable()
    {
        EventManager.OnPrimaryTargetDeactivated += HandlePrimaryTargetDeactivated;
    }

    private void OnDisable()
    {
        EventManager.OnPrimaryTargetDeactivated -= HandlePrimaryTargetDeactivated;
    }

    public override void Initialize()
    {
        base.Initialize();
        PlaceSpheresRandomly();
    }

    public void PlaceSpheresRandomly()
    {
        for (int i = 0; i < count; i++)
        {
            bool positionFound = false;
            Transform newTargetTransform = Instantiate(targetPrefab, targetsParentTransform).transform;

            float targetRadius = UnityEngine.Random.Range(minTargetRadius, maxTargetRadius);
            newTargetTransform.localScale = Vector3.one * (targetRadius * 2); // Set the scale of the sphere

            while (!positionFound)
            {
                Vector3 randomPosition = GetRandomPositionWithinBounds();

                newTargetTransform.localPosition = randomPosition;

                if (!IsOverlapping(newTargetTransform, targetRadius))
                {
                    targetPrefabList.Add(newTargetTransform);
                    newTargetTransform.GetComponent<GazeTarget>().IsPrimary = false;
                    positionFound = true;
                }
            }

            if (!positionFound)
            {
                Debug.LogWarning("Could not find a suitable position for a sphere after 100 attempts.");
                Destroy(newTargetTransform.gameObject);
            }
        }

        // Set one target to primary
        RandomPrimaryTarget();
    }

    private void HandlePrimaryTargetDeactivated(GazeTarget deactivatedTarget)
    {
        collectedTargets++;
        audioSource.Play();
        RandomPrimaryTarget();
    }

    public Vector3 GetRandomPositionWithinBounds()
    {
        float randomX = UnityEngine.Random.Range(minLimit.x, maxLimit.x);
        float randomY = UnityEngine.Random.Range(minLimit.y, maxLimit.y);

        return new Vector3(randomX, randomY, 0f);
    }

    public void DestroyAllTargets()
    {
        foreach (var target in targetPrefabList)
        {
            Destroy(target.gameObject);
        }
        targetPrefabList.Clear();
    }

    bool IsOverlapping(Transform newTarget, float radius)
    {
        foreach (Transform target in targetPrefabList)
        {
            if (SphereOverlaps(newTarget.position, radius, target.position, target.localScale.x / 2))
            {
                return true;
            }
        }
        return false;
    }

    bool SphereOverlaps(Vector3 center1, float radius1, Vector3 center2, float radius2)
    {
        float distance = Vector3.Distance(center1, center2);
        return distance < (radius1 + radius2);
    }

    public void RandomPrimaryTarget()
    {
        var index = UnityEngine.Random.Range(0, targetPrefabList.Count - 1);
        var gazeTarget = targetPrefabList[index].GetComponent<GazeTarget>();
        gazeTarget.IsPrimary = true;
        SimpleGameManager.Instance.SetTargetPosition(gazeTarget.transform.localPosition);
    }

    // This is caleld every frame
    public void UpdateCursorPosition(Vector2 position)
    {
        cursorTransform.localPosition = position;

        // Find the closest target
        if (targetPrefabList.Count > 0)
        {
            Transform closestTarget = FindClosestTarget();

            // Update the cursor's scale to match the closest target's scale
            if (closestTarget != null)
            {
                cursorTransform.localScale = Vector3.Lerp(cursorTransform.localScale, closestTarget.localScale, scaleLerpSpeed * Time.deltaTime);
            }

            // Check if we are close to the primary target
            if (CheckIfOnPrimary())
            {
                var gazeTarget = closestTarget.GetComponent<GazeTarget>();
                gazeTarget.StartInteraction();
            }
            else
            {
                var gazeTarget = closestTarget.GetComponent<GazeTarget>();
                gazeTarget.StopInteraction();
            }
        }
    }

    Transform FindClosestTarget()
    {
        Transform closestDummy = null;
        float minDistance = float.MaxValue;

        foreach (Transform target in targetPrefabList)
        {
            if (target != null)
            {
                float distance = Vector3.Distance(cursorTransform.localPosition, target.localPosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestDummy = target;
                }
            }
        }
        return closestDummy;
    }

    private bool CheckIfOnPrimary()
    {
        foreach (Transform target in targetPrefabList)
        {
            if (target != null)
            {
                var gazeTarget = target.GetComponent<GazeTarget>();
                if (gazeTarget != null && gazeTarget.IsPrimary)
                {
                    float distance = Vector3.Distance(cursorTransform.localPosition, target.localPosition);
                    if (distance < interactableDistance)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public int GetCollectedTargets()
    {
        return collectedTargets;
    }
}
