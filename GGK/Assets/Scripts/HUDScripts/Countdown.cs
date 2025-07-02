using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    // References
    public PauseHandler pauseHandler;

    public CanvasGroup startPanel;
    public TextMeshProUGUI countdownText;

    // Other
    public float countdownSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        // Pause game
        Time.timeScale = 0;

        // Start Countdown
        StartCoroutine(CountDown());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CountDown()
    {
        // Allow for fade out to complete before starting
        yield return new WaitForSecondsRealtime(countdownSpeed);

        // Shrink 3 down
        countdownText.rectTransform.DOScale(Vector3.zero, countdownSpeed).SetUpdate(true);

        yield return new WaitForSecondsRealtime(countdownSpeed);

        // Reset scale then shrink 2 down
        countdownText.text = "2";
        countdownText.rectTransform.localScale = Vector3.one;
        countdownText.rectTransform.DOScale(Vector3.zero, countdownSpeed).SetUpdate(true);

        yield return new WaitForSecondsRealtime(countdownSpeed);

        // Reset scale then shrink 1 down
        countdownText.text = "1";
        countdownText.rectTransform.localScale = Vector3.one;
        countdownText.rectTransform.DOScale(Vector3.zero, countdownSpeed).SetUpdate(true);

        yield return new WaitForSecondsRealtime(countdownSpeed);

        // Start game & allow pause panel to pulled up
        Time.timeScale = 1;
        pauseHandler.countdownDone = true;

        // Reset scale then shrink GO! down
        countdownText.text = "GO!";
        countdownText.rectTransform.localScale = Vector3.one;
        countdownText.rectTransform.DOScale(Vector3.zero, countdownSpeed);
    }
}
    