using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerOption : MonoBehaviour
{
    [SerializeField]
    string playerChoice;
    // Start is called before the first frame update

    public void onClick()
    {
        RacerDataManager.instance.RacerChoice = playerChoice;
        SceneManager.LoadScene("CharacterCustomization");
    }
}
