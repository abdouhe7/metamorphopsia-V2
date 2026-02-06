using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AmslerGridEyeTracking : BaseAssessmentController
{
    public float moveSpeed = 1f;     // Speed of movement
    public Transform target;
    public Transform backgroundPlane;

    private Vector2 currentTarget;   // Current target point
    private bool alternateDirection = false;

    public override void Reset()
    {
        StopAllCoroutines();
        currentTarget = Vector2.zero;
        target.localPosition = currentTarget;
        SimpleGameManager.Instance.SetTargetPosition(Vector3.zero);
    }

    public override void Initialize()
    {
        // Start at the center (or closest point to center)
        currentTarget = Vector2.zero;
        StartCoroutine(MoveToNextPoint());
        backgroundPlane.localScale = new Vector3(SimpleGameManager.Instance.width / 1000f, 1f, SimpleGameManager.Instance.height / 1000f);
        base.Initialize();
    }

    IEnumerator MoveToNextPoint()
    {
        while (true)
        {
            // Move towards the current target point
            target.localPosition = Vector2.MoveTowards(target.localPosition, currentTarget, moveSpeed * Time.deltaTime);

            // Check if we've reached the target
            if ((Vector2)target.localPosition == currentTarget)
            {
                target.localPosition = currentTarget;
                alternateDirection = !alternateDirection;
                // Find the next closest non-diagonal point
                Vector2 nextPoint = GetNextPoint(currentTarget, alternateDirection);

                // If a valid next point is found, set it as the new target
                if (nextPoint != currentTarget)
                {
                    currentTarget = nextPoint;
                }
                else
                {
                    // No valid next point, stop moving
                    break;
                }
            }

            SimpleGameManager.Instance.SetTargetPosition(target.localPosition);

            yield return null;
        }
    }

    private Vector2 GetNextPoint(Vector2 currentPoint, bool modifyXNext)
    {
        var incrementX = 0.96f * 3f; // Fixed increment value for X
        var incrementY = 0.5f * 3f; // Fixed increment value for Y

        // Define boundaries
        float minX = -9f;
        float maxX = 9f;
        float minY = -5f;
        float maxY = 5f;

        Vector2 nextPoint = currentPoint;

        if (modifyXNext)
        {
            // Randomly decide to increase or decrease
            bool increaseX = Random.value > 0.5f;
            float newX = currentPoint.x + (increaseX ? incrementX : -incrementX);

            // Check if the new x value is within bounds
            if (newX <= maxX && newX >= minX)
            {
                nextPoint.x = newX;
            }
            else
            {
                // If out of bounds, go the opposite direction
                newX = currentPoint.x + (increaseX ? -incrementX : incrementX);
                if (newX <= maxX && newX >= minX)
                {
                    nextPoint.x = newX;
                }
            }
        }
        else
        {
            // Randomly decide to increase or decrease
            bool increaseY = Random.value > 0.5f;
            float newY = currentPoint.y + (increaseY ? incrementY : -incrementY);

            // Check if the new y value is within bounds
            if (newY <= maxY && newY >= minY)
            {
                nextPoint.y = newY;
            }
            else
            {
                // If out of bounds, go the opposite direction
                newY = currentPoint.y + (increaseY ? -incrementY : incrementY);
                if (newY <= maxY && newY >= minY)
                {
                    nextPoint.y = newY;
                }
            }
        }

        return nextPoint;
    }

}
