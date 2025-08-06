using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeedAndTimeDisplay : MonoBehaviour
{
    // Fields for text displays
    [SerializeField]
    TextMeshProUGUI speedDisplay;
    [SerializeField]
    TextMeshProUGUI timeDisplay;

    [SerializeField]
    NEWDriver kart;

    [SerializeField]
    SpeedBar speedBar;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        speedBar.SetMaxSpeed(150);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        float seconds = timer % 60;
        int minutes = (int)timer / 60;
        speedBar.SetSpeed(kart.sphere.velocity.magnitude);
        speedDisplay.text = "Speed: " + kart.sphere.velocity.magnitude.ToString("n2");
        timeDisplay.text = "Time: " + string.Format("{0:00}:{1:00.00}", minutes, seconds);
    }
}
