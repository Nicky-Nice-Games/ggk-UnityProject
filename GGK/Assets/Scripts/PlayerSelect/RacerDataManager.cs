using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerDataManager : MonoBehaviour
{
    public static RacerDataManager instance;

    public RacerDataManager Instance { get { return instance; } }

    private string racerChoice;
    public string RacerChoice { get { return racerChoice; } set { racerChoice = value; } }
    private Material colorChoice;
    public Material ColorChoice { get { return colorChoice; } set { colorChoice = value; } }

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
