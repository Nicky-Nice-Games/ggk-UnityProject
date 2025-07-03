using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

    // Start is called before the first frame update
    void Start()
    {
        // Find karts and turn off their ability to drive
        karts = FindObjectsByType<KartCheckpoint>(FindObjectsSortMode.None).ToList();

        foreach (KartCheckpoint kart in karts)
        {
            if (kart.gameObject.GetComponent<NPCDriver>() != null)
            {
                kart.gameObject.GetComponent<NPCDriver>().enabled = false;
            }
            else
            {
                kart.transform.parent.GetChild(0).GetComponent<NEWDriver>().enabled = false;
            }
        }

        // Start Countdown
        StartCoroutine(CountDown());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CountDown()
    {
        countdownText.rectTransform.DOScale(Vector3.zero, countdownSpeed);

        yield return new WaitForSeconds(countdownSpeed);

        countdownText.text = "2";
        countdownText.rectTransform.localScale = Vector3.one;
        countdownText.rectTransform.DOScale(Vector3.zero, countdownSpeed);

        yield return new WaitForSeconds(countdownSpeed);

        countdownText.text = "1";
        countdownText.rectTransform.localScale = Vector3.one;
        countdownText.rectTransform.DOScale(Vector3.zero, countdownSpeed);

        yield return new WaitForSeconds(countdownSpeed);

        // turn ability to drive back on
        foreach (KartCheckpoint kart in karts)
        {
            if (kart.gameObject.GetComponent<NPCDriver>() != null)
            {
                kart.gameObject.GetComponent<NPCDriver>().enabled = true;
            }
            else
            {
                kart.transform.parent.GetChild(0).GetComponent<NEWDriver>().enabled = true;
            }
        }

        countdownText.text = "GO!";
        countdownText.rectTransform.localScale = Vector3.one;
        countdownText.rectTransform.DOScale(Vector3.zero, countdownSpeed);
    }
}
    