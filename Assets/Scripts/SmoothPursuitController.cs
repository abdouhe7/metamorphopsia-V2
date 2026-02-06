using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothPursuitController : BaseAssessmentController
{
    public Transform target;
    public float radius = 5f;
    public float speed = 1f;
    public bool clockwise = false;
    
    private float angle = 0f;

    public override void Reset()
    {
        base.Reset();
        angle = 0f;
        target.localPosition = Vector3.zero;
    }

    public override void Initialize()
    {
        base.Initialize();
        StartCoroutine(SmoothPursuit());
    }

    private IEnumerator SmoothPursuit()
    {
        
        while (true)
        {
            angle += (clockwise ? -1 : 1) * speed * Time.deltaTime;

            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);

            target.localPosition = new Vector3(x, y, 0f);

            SimpleGameManager.Instance.SetTargetPosition(target.localPosition);

            yield return null;
        }
    }
}
