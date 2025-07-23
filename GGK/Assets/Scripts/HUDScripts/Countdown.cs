using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Countdown : NetworkBehaviour
{
    /*
    //// List of karts to stop moving while countdown is occuring
    //List<KartCheckpoint> karts;

    // Referneces
    public TextMeshProUGUI countdownText;

    // Other
    public int countdownCount = 3;
    public float countdownSpeed = 1f;
    public bool finished = false;

    public NetworkVariable<int> netCountdownCount = new NetworkVariable<int>(3);
    public NetworkVariable<bool> netFinished = new NetworkVariable<bool>(false);

    // Start is called before the first frame update
    void Start()
    {
        // Pause stuff using scaled time
        Time.timeScale = 0;

        {
            //// Find karts and turn off their ability to drive
            //karts = FindObjectsByType<KartCheckpoint>(FindObjectsSortMode.None).ToList();

            //foreach (KartCheckpoint kart in karts)
            //{
            //    if (kart.gameObject.GetComponent<NPCDriver>() != null)
            //    {
            //        kart.gameObject.GetComponent<NPCDriver>().enabled = false;
            //    }
            //    else
            //    {
            //        kart.transform.parent.GetChild(0).GetComponent<NEWDriver>().enabled = false;
            //    }
            //}
        }


        if (!IsSpawned)
        {
            // Start Countdown
            StartCoroutine(CountDown());
        }
        // Multiplayer
        else if (IsServer)
        {
            CountdownStartRpc();
        }
    }


    public IEnumerator CountDown()
    {
        if (!IsSpawned)
        {
            // Change text depending on countdownCount
            if (countdownCount > 0)
            {
                countdownText.text = countdownCount.ToString();
            }
            else
            {
                // if countdownCount is 0 or less then start race!
                countdownText.text = "GO!";
                Time.timeScale = 1;
                finished = true;
            }

            // Expand and shrink text
            countdownText.rectTransform.DOScale(Vector3.one, countdownSpeed / 3).SetUpdate(true).OnComplete(() =>
            {
                countdownText.rectTransform.DOScale(Vector3.zero, countdownSpeed / 3).SetUpdate(true).SetDelay(countdownSpeed / 3);
            });

            yield return new WaitForSecondsRealtime(countdownSpeed);

            // if countdownCount is greater than 0 then decrement it and call CountDown again
            if (countdownCount > 0)
            {
                countdownCount -= 1;
                StartCoroutine(CountDown());
            }
        }
        else if(IsServer)
        {
            UpdateCountdownRpc();
        }

        {
            //// turn ability to drive back on
            //foreach (KartCheckpoint kart in karts)
            //{
            //    if (kart.gameObject.GetComponent<NPCDriver>() != null)
            //    {
            //        kart.gameObject.GetComponent<NPCDriver>().enabled = true;
            //    }
            //    else
            //    {
            //        kart.transform.parent.GetChild(0).GetComponent<NEWDriver>().enabled = true;
            //    }
            //}
        }
    }

    /// <summary>
    /// Rpc to start countdown across clients and host
    /// </summary>
    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    public void CountdownStartRpc()
    {
        StartCoroutine(CountDown());
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    public void UpdateCountdownRpc()
    {
        // Change text depending on countdownCount
        if (netCountdownCount.Value > 0)
        {
            countdownText.text = netCountdownCount.Value.ToString();
        }
        else
        {
            // if countdownCount is 0 or less then start race!
            countdownText.text = "GO!";
            Time.timeScale = 1;
            netFinished.Value = true;
        }

        // Expand and shrink text
        countdownText.rectTransform.DOScale(Vector3.one, countdownSpeed / 3).SetUpdate(true).OnComplete(() =>
        {
            countdownText.rectTransform.DOScale(Vector3.zero, countdownSpeed / 3).SetUpdate(true).SetDelay(countdownSpeed / 3);
        });

        StartCoroutine(WaitPlease());

        // if countdownCount is greater than 0 then decrement it and call CountDown again
        if (netCountdownCount.Value > 0)
        {
            netCountdownCount.Value -= 1;
            StartCoroutine(CountDown());
        }
    }

    IEnumerator WaitPlease()
    {
        yield return new WaitForSecondsRealtime(countdownSpeed);
    }
    */

    //// List of karts to stop moving while countdown is occuring
    //List<KartCheckpoint> karts;

    // References
    public TextMeshProUGUI countdownText;

    // Other
    public int countdownStart = 3;
    public float countdownSpeed = 1f;
    public bool finished = false;

    // Start is called before the first frame update
    void Start()
    {
        // Pause stuff using scaled time
        Time.timeScale = 0;

        // Host in Multiplayer 
        if (IsServer && NetworkManager.Singleton.IsListening)
        {
            StartCoroutine(ServerCountdownRoutine());
        }

        // Singleplayer
        else if (!NetworkManager.Singleton.IsListening)
        {
            StartCoroutine(LocalCountdownRoutine());
        }

        //// Find karts and turn off their ability to drive
        //karts = FindObjectsByType<KartCheckpoint>(FindObjectsSortMode.None).ToList();

        //foreach (KartCheckpoint kart in karts)
        //{
        //    if (kart.gameObject.GetComponent<NPCDriver>() != null)
        //    {
        //        kart.gameObject.GetComponent<NPCDriver>().enabled = false;
        //    }
        //    else
        //    {
        //        kart.transform.parent.GetChild(0).GetComponent<NEWDriver>().enabled = false;
        //    }
        //}
    }

    IEnumerator ServerCountdownRoutine()
    {
        yield return new WaitForSecondsRealtime(countdownSpeed);

        int count = countdownStart;

        while (count > 0)
        {
            UpdateCountdownRpc(count);
            yield return new WaitForSecondsRealtime(countdownSpeed);
            count--;
        }

        UpdateCountdownRpc(0); // Show "GO!"
        EndCountdownRpc();
        yield return new WaitForSecondsRealtime(countdownSpeed);
    }

    IEnumerator LocalCountdownRoutine()
    {
        yield return new WaitForSecondsRealtime(countdownSpeed);

        int count = countdownStart;

        while (count > 0)
        {
            ShowCountdown(count);
            yield return new WaitForSecondsRealtime(countdownSpeed);
            count--;
        }

        ShowCountdown(0); // Show "GO!"
        FinishCountdown();
        yield return new WaitForSecondsRealtime(countdownSpeed);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void UpdateCountdownRpc(int count)
    {
        ShowCountdown(count);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void EndCountdownRpc()
    {
        FinishCountdown();
    }

    void ShowCountdown(int count)
    {
        countdownText.text = count > 0 ? count.ToString() : "GO!";

        countdownText.rectTransform.localScale = Vector3.zero;
        countdownText.rectTransform.DOScale(Vector3.one, countdownSpeed / 3).SetUpdate(true).OnComplete(() =>
        {
            countdownText.rectTransform.DOScale(Vector3.zero, countdownSpeed / 3).SetUpdate(true).SetDelay(countdownSpeed / 3);
        });
    }

    void FinishCountdown()
    {
        Time.timeScale = 1;
        finished = true;

        //// turn ability to drive back on
        //foreach (KartCheckpoint kart in karts)
        //{
        //    if (kart.gameObject.GetComponent<NPCDriver>() != null)
        //    {
        //        kart.gameObject.GetComponent<NPCDriver>().enabled = true;
        //    }
        //    else
        //    {
        //        kart.transform.parent.GetChild(0).GetComponent<NEWDriver>().enabled = true;
        //    }
        //}
    }
}
