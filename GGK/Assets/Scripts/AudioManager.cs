using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    private AudioSource musicPlayer;

    [SerializeField]
    List<string> stopSceneNames;

    // Reference to other scripts
    private SoundVolume soundVolume;
    private OptionsData optionsData;
    private float curMasterVolume;
    private float curMusicVolume;

    private void Awake()
    {
        // only have one instance of this script at a time
        if (instance == null)
        {
            instance = this;

            // persists across scenes
            DontDestroyOnLoad(gameObject);

            musicPlayer = GetComponent<AudioSource>();

            // subscribes to OnSceneLoaded method to check when a scene is started
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            // destroy this script if one already exists
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        soundVolume = FindAnyObjectByType<SoundVolume>();
        optionsData = soundVolume.optionsData;
        curMasterVolume = optionsData.masterVolume;
        curMusicVolume = optionsData.musicVolume;

        // play menu music is audio source exists and isnt playing
        if (musicPlayer != null && !musicPlayer.isPlaying)
        {
            soundVolume.PlayMusic(musicPlayer);
        }
    }

    private void Update()
    {
        // change volume if master or music volume is changed
        if (curMasterVolume != optionsData.masterVolume || curMusicVolume != optionsData.musicVolume)
        {
            curMasterVolume = optionsData.masterVolume;
            curMusicVolume = optionsData.musicVolume;

            soundVolume.ChangeMusicVolume(musicPlayer);
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
            soundVolume.PlayMusic(musicPlayer);
        }
    }

    private void OnDestroy()
    {
        // unsubscribe when destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
