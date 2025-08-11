using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : NetworkBehaviour
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
        StartAnimation();

        // Wait
        if (transitionActive)
            yield return new WaitForSeconds(transitionTime);

        // Load Scene and play end scene transition animation
        SceneManager.LoadScene(sceneName);
        EndAnimation();
    }

    public IEnumerator ServerLoadScene(string sceneName)
    {
        if (IsServer && SceneManager.GetActiveScene().name != sceneName)
        {
            StartAnimationRpc();
            if (transitionActive) yield return new WaitForSeconds(transitionTime);
        
            SceneEventProgressStatus status = NetworkManager.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to load {sceneName} with a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }
       
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void StartAnimationRpc()
    {
        StartAnimation();
    }

    public void StartAnimation()
    {
        // Play start scene transition animation
        if (transitionActive && loading == false) transition.SetTrigger("Start");
    }

    public void EndAnimation()
    {
        if (transitionActive)
        {
            transition.SetTrigger("End");
            loading = false;
        }

    }
}
