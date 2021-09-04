using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour
{

    private LineRenderer lineRenderer;
    private Transform[] points;
    
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material.renderQueue = 0;

        if (points == null)
        {
            lineRenderer.enabled = false;
        }
    }

    public void SetUpLine(Transform[] points)
    {
        lineRenderer.positionCount = points.Length;
        this.points = points;
    }

    private void Update()
    {
        if (points != null)
        {
            for (var index = 0; index < points.Length; index++)
            {
                lineRenderer.SetPosition(index, points[index].position);
            }
            
            lineRenderer.enabled = true;
        }
    }
}
