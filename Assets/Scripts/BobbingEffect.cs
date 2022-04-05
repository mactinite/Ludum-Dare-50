using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BobbingEffect : MonoBehaviour
{

    public bool renderLine;
    [ShowIf("renderLine")]
    public LineRenderer lineRenderer;

    public float distance = 5;
    public float moveTime = 1;
    public Vector3 originalPosition = Vector3.zero;
    public Vector3 direction = Vector3.up;
    public AnimationCurve easing;

    float timer = 0;
    [Range(0,1)]
    public float startPosition = 0;
    bool flip = false;
    [ChildGameObjectsOnly]
    public Transform bobber;

    // Start is called before the first frame update
    void Start()
    {
        timer = startPosition;
    }

    private void OnEnable()
    {
        timer = startPosition;
        flip = false;

        if (renderLine)
        {
            lineRenderer.SetPositions(new Vector3[]{ transform.position, transform.position + direction * distance });
        }
    }

    // Update is called once per frame
    void Update()
    {


        if (!flip && timer >= moveTime)
        {
            flip = true;
        } 

        if(flip && timer < 0)
        {
            flip = false;
        }

        timer = flip ? timer - Time.deltaTime : timer + Time.deltaTime;


        bobber.localPosition = new Vector3(bobber.localPosition.x, originalPosition.y + easing.Evaluate(timer/moveTime) * distance, bobber.localPosition.z);
    }

    private void OnDrawGizmos()
    {
        Vector3 startPosition = transform.TransformPoint(originalPosition);
        Vector3 endPosition = transform.TransformPoint(originalPosition + (Vector3.up * (distance)));
        Gizmos.DrawLine(startPosition, endPosition);
    }

    private void OnValidate()
    {
        bobber.localPosition = new Vector3(bobber.localPosition.x, originalPosition.y + easing.Evaluate(startPosition * moveTime / moveTime) * distance, bobber.localPosition.z);
        if (renderLine)
        {
            lineRenderer.SetPositions(new Vector3[] { transform.position, transform.position + direction * distance });
        }
    }
}
