using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DevTools : MonoBehaviour
{

    public static DevTools Instance;
    private List<GameObject> listeners = new List<GameObject>();   


    // Start is called before the first frame update
    void Start()
    {
        AddListener(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }



    public void AddListener(GameObject listener)
    {

        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }


    public void Command(string input)
    {

        //string[] parts = input.Split(new char[] {

    }


}
