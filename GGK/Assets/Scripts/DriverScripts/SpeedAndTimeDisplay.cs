using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// THIS SCRIPT IS NOW OUT OF DATE AS THE SPEEDOMETER HAS BEEN SCRAPPED
// TIME DISPLAY HAS BEEN MOVED TO LEADERBOARD CONTROLLER
public class SpeedAndTimeDisplay : MonoBehaviour
{
    public static SpeedAndTimeDisplay instance;

    // Fields for text displays
    [SerializeField]
    TextMeshProUGUI speedDisplay;

    // [SerializeField]
    // TextMeshProUGUI timeDisplay;

    [SerializeField]
    public NEWDriver kart;

    [SerializeField]
    SpeedBar speedBar;
    float timer;

    private void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        speedBar.SetMaxSpeed(150);
    }

    // Update is called once per frame
    void Update()
    {
        //speedBar.SetSpeed(kart.sphere.velocity.magnitude);
        //speedDisplay.text = "Speed: " + kart.sphere.velocity.magnitude.ToString("n2");
    }

    public void TrackKart(GameObject trackedKart)
    {
        kart = trackedKart.GetComponent<NEWDriver>();
    }
}
