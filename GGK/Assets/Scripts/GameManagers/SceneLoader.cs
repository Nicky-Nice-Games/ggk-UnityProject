using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime;
    public bool transitionActive;

    public bool loading = false;

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadAnimation(sceneName));
        loading = true;
    }

    IEnumerator LoadAnimation(string sceneName)
    {
        // Play start scene transition animation
        if (transitionActive)
            transition.SetTrigger("Start");

        // Wait
        if (transitionActive)
            yield return new WaitForSeconds(transitionTime);

        // Load Scene and play end scene transition animation
        SceneManager.LoadScene(sceneName);
        if (transitionActive)
            transition.SetTrigger("End");  
    }
}
