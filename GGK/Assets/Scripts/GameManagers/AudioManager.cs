using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    AudioSource musicPlayer;

    [SerializeField]
    List<string> stopSceneNames;

    private void Awake()
    {
        // only have one instance of this script at a time
        if (instance == null)
        {
            instance = this;

            // persists across scenes
            DontDestroyOnLoad(gameObject);


            musicPlayer = GetComponent<AudioSource>();

            // play menu music is audio source exists and isnt playing
            if (musicPlayer != null && !musicPlayer.isPlaying)
            {
                musicPlayer.Play();
            }

            // subscribes to OnSceneLoaded method to check when a scene is started
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            // destroy this script if one already exists
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // stops menu music when given scene starts
        for (int x = 0; x < stopSceneNames.Count; x++)
        {
            if (scene.name == stopSceneNames[x])
            {
                musicPlayer.Stop();
                return;
            }
        }

        // starts music in other scenes if it stops
        if (!musicPlayer.isPlaying)
        {
            musicPlayer.Play();
        }
    }

    private void OnDestroy()
    {
        // unsubscribe when destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
