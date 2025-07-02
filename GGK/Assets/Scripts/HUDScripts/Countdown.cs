using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    // List of karts to stop moving while countdown is occuring
    List<KartCheckpoint> karts;

    // Referneces
    public CanvasGroup startPanel;
    public TextMeshProUGUI countdownText;

    // Other
    public float countdownSpeed = 1f;
    public bool counting;

    // Start is called before the first frame update
    void Start()
    {
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
        counting = true;

        countdownText.rectTransform.DOScale(Vector3.zero, countdownSpeed).SetUpdate(true);

        yield return new WaitForSecondsRealtime(countdownSpeed);

        countdownText.text = "2";
        countdownText.rectTransform.localScale = Vector3.one;
        countdownText.rectTransform.DOScale(Vector3.zero, countdownSpeed).SetUpdate(true);

        yield return new WaitForSecondsRealtime(countdownSpeed);

        countdownText.text = "1";
        countdownText.rectTransform.localScale = Vector3.one;
        countdownText.rectTransform.DOScale(Vector3.zero, countdownSpeed).SetUpdate(true);

        yield return new WaitForSecondsRealtime(countdownSpeed);

        Time.timeScale = 1;
        counting = false;

        countdownText.text = "GO!";
        countdownText.rectTransform.localScale = Vector3.one;
        countdownText.rectTransform.DOScale(Vector3.zero, countdownSpeed);
    }
}
    