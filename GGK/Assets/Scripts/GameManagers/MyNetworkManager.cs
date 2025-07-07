using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkManager : MonoBehaviour
{
    public static MyNetworkManager Instance;

    private void Awake() {
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(this);
        } else if (Instance != this){
            Destroy(gameObject);
        }
    }
}
