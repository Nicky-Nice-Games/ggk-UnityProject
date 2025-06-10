using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineBranching : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private SplineAnimate splineAnimate;
    [SerializeField] private SplineContainer mainSplineContainer;
    [SerializeField] private SplineContainer shortcutSplineContainer;
    [SerializeField] private SplineContainer secondMainSpline;
    [SerializeField] private float returnToMainSplineTime = 0.55f;
    [SerializeField] private float returnTo2ndSplineTime = 0.07f;
    [SerializeField] private float returnTo1stSplineTime = 0.045f;
    [SerializeField] private SplineContainer beforeEnterShortcut;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (splineAnimate.Container != shortcutSplineContainer)
        {
            beforeEnterShortcut = splineAnimate.Container;
        }
    }



    public void TakeShortcut()
    {

        splineAnimate.Container = shortcutSplineContainer;
        splineAnimate.Restart(true);

    }

    public void ReturnToMain()
    {
        if (splineAnimate.Container != beforeEnterShortcut)
        {
            splineAnimate.Container = beforeEnterShortcut;
            splineAnimate.NormalizedTime = returnToMainSplineTime;
        }
    }

    public void switchMain()
    {
        Debug.Log("Switch Main Spline");
        if (splineAnimate.Container != secondMainSpline)
        {
            splineAnimate.Container = secondMainSpline;
            splineAnimate.NormalizedTime = returnTo2ndSplineTime;
        }
        else if (splineAnimate.Container != mainSplineContainer)
        {
            splineAnimate.Container = mainSplineContainer;
            splineAnimate.NormalizedTime = returnTo1stSplineTime;
        }
    }
}
