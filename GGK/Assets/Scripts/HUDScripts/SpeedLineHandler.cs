using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedLineHandler : MonoBehaviour
{
    [SerializeField, Tooltip("Reference to the speedline GameObject")]
    private Image speedlineReference;
    [SerializeField, Tooltip("Maximum amount of lines in circulation allowed")]
    private int maxLines;

    [SerializeField, Tooltip("How long it takes for a speedline to spawn")]
    private float frequency;
    [SerializeField, Tooltip("How varied the speedlines spawning are. value of 2 = min 0.5x max 2x")]
    private float variance;
    [SerializeField, Tooltip("The amount of time it takes for a single speedline to expire")]
    private float expireTime;
    [SerializeField, Tooltip("the radius of the circle that the speedlines enclose")]
    private float radius;

    [SerializeField, Tooltip("The width/height multiplier of the speedlines' positions in the radius")]
    private Vector2 oval;

    [SerializeField, Tooltip("The percentage of the player's max speed at which speed lines will appear. value of 0 = speed lines will always appear when accelerating")]
    float speedThreshold;

    [SerializeField, Tooltip("Reference to the player the canvas is following")]
    NEWDriver trackingPlayer;


    private List<Image> activeLines = new List<Image>();
    private List<Image> inactiveLines = new List<Image>();

    private float timer;

    private float minTimer, maxTimer;
    // Start is called before the first frame update
    void Start()
    {
        inactiveLines.Add(speedlineReference);
        minTimer = frequency/variance;
        maxTimer = frequency*variance;

        timer = Random.Range(minTimer, maxTimer);
    }

    // Update is called once per frame
    void Update()
    {
        //if the player is approaching max speed, begin spawning speed lines to
        //indicate
        if (trackingPlayer.sphere.velocity.magnitude > (trackingPlayer.maxSpeed * 0.95))
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                SpawnLine();
                timer = Random.Range(minTimer, maxTimer) / (float)(trackingPlayer.sphere.velocity.magnitude / (trackingPlayer.maxSpeed * 0.95));
            }
        }

        

        for(int i = 0; i < activeLines.Count; i++){
            activeLines[i].rectTransform.localScale -= new Vector3(1/expireTime * Time.deltaTime, 0, 0);

            if (activeLines[i].rectTransform.localScale.x <= 0){
                SetInactive(activeLines[i]);
            }
        }
    }


    private void SpawnLine()
    {
        //don't add a new line if the maximum amount of lines have been spawned
        if((activeLines.Count + inactiveLines.Count) < maxLines){
            GameObject newline;

            if(inactiveLines.Count > 0){
                newline = inactiveLines[0].gameObject;
            }
            else{
                newline = Instantiate(speedlineReference.gameObject, speedlineReference.transform.parent);
            }

            Image newImg = newline.GetComponent<Image>();
            SetActive(newImg);
            Reposition(newImg);
        }
    }

    public void Reposition(Image img){
        float randomRot = Random.Range(0, 360);
        float radians = randomRot * (Mathf.PI / 180);
        RectTransform tr = img.rectTransform;
        tr.rotation = Quaternion.Euler(0, 0, randomRot);

        tr.localPosition = new Vector3(radius * Mathf.Cos(radians) * oval.x, radius * Mathf.Sin(radians) * oval.y, 0);
    }

    public void SetActive(Image img){
        if(inactiveLines.Contains(img)){
            inactiveLines.Remove(img);
        }
        activeLines.Add(img);
        img.rectTransform.localScale = new Vector3(1, 1, 1);
        img.gameObject.SetActive(true);

    }

    public void SetInactive(Image img){
        if(activeLines.Contains(img)){
            activeLines.Remove(img);
        }
        inactiveLines.Add(img);
        img.gameObject.SetActive(false);
    }
}
