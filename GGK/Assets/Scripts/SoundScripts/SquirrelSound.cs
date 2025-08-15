using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquirrelSound : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event Squirrels;

    bool playSquirrels = true;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Kart") && playSquirrels)
        {
            PlaySquirrels();
            playSquirrels = false;
            StartCoroutine(SquirrelRandom());
        }
    }

    IEnumerator SquirrelRandom()
    {
        int randNum = Random.Range(1, 3);
        yield return new WaitForSeconds(randNum);
        playSquirrels = true;
    }

    public void PlaySquirrels()
    {
        Squirrels.Post(gameObject);
    }
}
