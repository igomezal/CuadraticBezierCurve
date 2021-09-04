using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class Line : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    
    [SerializeField] private LineRendererController lineRendererController;
    [SerializeField] private LineRendererController blueLineRendererController;
    [SerializeField] private LineRendererController quadraticBezier;
    
    [SerializeField] private GameObject blueGreenDotPrefab;
    [SerializeField] private GameObject babyBlueDotPrefab;
    
    [SerializeField] private int interpolationFramesCount = 700; // Number of frames to completely interpolate between the 2 positions
    
    private int elapsedFrames = 0;
    private int goal;
    
    private List<GameObject> blueGreenDots = new List<GameObject>();
    private GameObject babyBlueDot;

    private bool bezierCreated = false;
    private List<GameObject> bezierPoints = new List<GameObject>();

    private void Start()
    {
        goal = interpolationFramesCount;
        
        lineRendererController.SetUpLine(points);

        for (var index = 0; index < points.Length - 1; index++)
        {
            var blueGreenDot = Instantiate(blueGreenDotPrefab, points[index].position, Quaternion.identity);
            blueGreenDot.GetComponent<SpriteRenderer>().material.renderQueue = 3000;
            blueGreenDots.Add(blueGreenDot);
        }

        blueLineRendererController.SetUpLine(blueGreenDots.Select(blueDot => blueDot.transform).ToArray());

        babyBlueDot = Instantiate(babyBlueDotPrefab, blueGreenDots[0].transform.position, Quaternion.identity);
        babyBlueDot.GetComponent<SpriteRenderer>().material.renderQueue = 3002;
        
        Color color;

        if (ColorUtility.TryParseHtmlString("#ffa602", out color))
        {
            babyBlueDot.GetComponent<SpriteRenderer>().material.color = color;
        }
        
        bezierPoints.Add(Instantiate(babyBlueDotPrefab, babyBlueDot.transform.position, Quaternion.identity));
    }

    private void Update()
    {
        float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;
        
        for (var index = 0; index < blueGreenDots.Count; index++)
        {
            blueGreenDots[index].transform.position = Vector3.Lerp(points[index].position, points[index + 1].position, interpolationRatio);
        }

        // Only create bezierPoints if there are two blue points (For QuadraticBezierCurve)
        if (blueGreenDots.Count == 2)
        {
            babyBlueDot.transform.position = Vector3.Lerp(blueGreenDots[0].transform.position, blueGreenDots[1].transform.position,
                interpolationRatio);
            if (!bezierCreated)
            {
                var bezierPoint = Instantiate(babyBlueDotPrefab, babyBlueDot.transform.position, Quaternion.identity);
                bezierPoint.GetComponent<SpriteRenderer>().material.renderQueue = 3001;
                bezierPoints.Add(bezierPoint);
            }
        }

        CalculateNewElapsedFrame();

        CheckLerpDirection();
    }

    private void CalculateNewElapsedFrame()
    {
        if (elapsedFrames > goal)
        {
            elapsedFrames = elapsedFrames - 1;

            CreateQuadraticBezierCurveFromBezierPoints();
        }
        else
        {
            elapsedFrames = elapsedFrames + 1;
        }
    }

    private void CreateQuadraticBezierCurveFromBezierPoints()
    {
        if (!bezierCreated)
        {
            quadraticBezier.SetUpLine(bezierPoints.Select(bezierPoint => bezierPoint.transform).ToArray());
            bezierCreated = true;
        }
    }

    private void CheckLerpDirection()
    {
        if (elapsedFrames == interpolationFramesCount)
        {
            goal = 0;
        }

        if (elapsedFrames == 0)
        {
            goal = interpolationFramesCount;
        }
    }
}
