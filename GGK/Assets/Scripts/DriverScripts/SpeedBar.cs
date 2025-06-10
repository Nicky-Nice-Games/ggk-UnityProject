// Joshua Chisholm
// 5/28/25
// Speedbar!
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedBar : MonoBehaviour
{
    [SerializeField]
    Slider speedBar;


    /// <summary>
    /// Sets the max value of the speed bar 
    /// and sets inital value to 0
    /// </summary>
    /// <param name="speed">Max speed</param>
    public void SetMaxSpeed(float speed)
    {
        speedBar.maxValue = speed;
        speedBar.value = 0;
    }

    /// <summary>
    /// Set the current value of the bar to a 
    /// specified speed
    /// </summary>
    /// <param name="speed">new speed</param>
    public void SetSpeed(float speed)
    {
        speedBar.value = speed;
    }
}