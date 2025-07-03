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


    /// <summary>
    /// Singleton functionality for promt to remain across scenes.
    /// </summary>
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


    /// <summary>
    /// Adds a game object as a listner if it is not already in the list.
    /// </summary>
    /// <param name="listener"></param>
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
