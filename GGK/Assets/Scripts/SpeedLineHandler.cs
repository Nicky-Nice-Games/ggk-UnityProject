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

    [SerializeField, Tooltip("The percentage of the player's max speed at which speed lines will appear. value of 0 = speed lines will always appear when accelerating")]
    float speedThreshold;

    [SerializeField, Tooltip("Reference to the player the canvas is following")]
    NEWDriver trackingPlayer;

    [SerializeField, Tooltip("If not set to anything, will automatically set this to its parent")]
    private Canvas canvas;
    [SerializeField, Tooltip("If not set to anything, will automatically set this to Camera.main")]
    private Camera mainCamera;
    private List<Image> activeLines = new List<Image>();
    private List<Image> inactiveLines = new List<Image>();
    private float radius;
    private Vector2 oval;
    private float timer;
    private float minTimer, maxTimer;
    private Vector3 playerPos;
    private float speedDiff;
    private float lastArea;
    private float normalFOVMax;
    private SpeedCameraEffect cameraEffect;
    // Start is called before the first frame update
    void Start()
    {

        if (canvas == null)
        {
            canvas = transform.parent.gameObject.GetComponent<Canvas>();
        }

        if (mainCamera == null){
            mainCamera = Camera.main;
            cameraEffect = mainCamera.GetComponent<SpeedCameraEffect>();
            normalFOVMax = cameraEffect.maxFOV;
        }

        inactiveLines.Add(speedlineReference);
        minTimer = frequency/variance;
        maxTimer = frequency*variance;

        timer = Random.Range(minTimer, maxTimer);
        RepositionLineRadius();
    }

    // Update is called once per frame
    void Update()
    {
        //if screen is resized
        if(lastArea != Screen.width * Screen.height){
            RepositionLineRadius();
        }

        playerPos = Camera.main.WorldToScreenPoint(trackingPlayer.sphere.transform.position);
        playerPos.z = 0;

        //if the player is past speed threshold, begin spawning speed lines to
        //indicate
        if (trackingPlayer.sphere.velocity.magnitude > (trackingPlayer.maxSpeed * speedThreshold))
        {
            speedDiff = trackingPlayer.sphere.velocity.magnitude / (trackingPlayer.maxSpeed * speedThreshold);
            timer -= Time.deltaTime;

            if(cameraEffect){
                cameraEffect.maxFOV = normalFOVMax * Mathf.Min(1.15f, speedDiff);
            }

            if (timer <= 0)
            {
                SpawnLine();
                timer = Random.Range(minTimer, maxTimer) / speedDiff;
            }
        }
        else{
            if(cameraEffect.maxFOV != normalFOVMax){
                cameraEffect.maxFOV = normalFOVMax;
            }
        }

        

        for(int i = 0; i < activeLines.Count; i++){
            activeLines[i].rectTransform.localScale -= new Vector3((1/expireTime * Time.deltaTime)/speedDiff, 0, 0);

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

    public void Reposition(Image img)
    {
        float randomRot = Random.Range(0, 360);
        float radians = randomRot * (Mathf.PI / 180);
        RectTransform tr = img.rectTransform;
        tr.rotation = Quaternion.Euler(0, 0, randomRot);

        tr.localPosition = new Vector3(radius * Mathf.Cos(radians) * oval.x, radius * Mathf.Sin(radians) * oval.y, 0);

        float speedLineModifier = Mathf.Min(1.25f, speedDiff);
        tr.sizeDelta = new Vector2(Screen.width * 0.75f * speedLineModifier, tr.sizeDelta.y);

        img.color = new Color (255, 255, 255, 0.05f * (speedLineModifier * 5));
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

    public void RepositionLineRadius(){

        if (Screen.width > Screen.height){
            radius = Screen.width * 2;

            oval.x = 1;
            oval.y = (float)Screen.height/(float)Screen.width;

        }
        else{
            radius = Screen.height * 2;

            oval.x = (float)Screen.width/(float)Screen.height;
            oval.y = 1;
        }

        lastArea = Screen.width*Screen.height;
    }
}
