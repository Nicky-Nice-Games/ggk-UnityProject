using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime;

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadAnimation(sceneName));
    }

    IEnumerator LoadAnimation(string sceneName)
    {
        // Play start scene transition animation
        transition.SetTrigger("Start");

        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Load Scene and play end scene transition animation
        SceneManager.LoadScene(sceneName);
        transition.SetTrigger("End");  
    }
}
