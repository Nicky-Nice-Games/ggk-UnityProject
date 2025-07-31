using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trigger Wwise sound events for character voice lines.
/// </summary>
public class VoiceLines : MonoBehaviour
{
    //The name of the character being played as.
    string characterName;

    [Header("Wwise Character States")]
    [SerializeField] AK.Wwise.State Emma;
    [SerializeField] AK.Wwise.State Gizmo;
    [SerializeField] AK.Wwise.State Jamster;
    [SerializeField] AK.Wwise.State Kai;
    [SerializeField] AK.Wwise.State Morgan;
    [SerializeField] AK.Wwise.State None;
    [SerializeField] AK.Wwise.State Reese;
    uint hitCollisionID = 0;
    uint itemThrownID = 0;
    uint lapMadeID = 0;
    uint racerPassed = 0;

    // Start is called before the first frame update
    void Start()
    {
        //The name of the current character is retrieved
        //from AppearanceSettings on the current GameObject.
        characterName = GetComponent<AppearanceSettings>().name;
    }

    public void PlayItemUsed()
    {
        //Depending on what the name of the character is, the voice lines used are different.
        //Thus, the specific event for that character and this circumstance is called.
        //Same structure with the following methods.
        if(characterName == "morgan")
        {

        }
        else if(characterName == "emma")
        {
            
        }
        else if(characterName == "kai")
        {
            
        }
        else if(characterName == "reese")
        {
            
        }
        else if(characterName == "jamster")
        {
            
        }
        else if(characterName == "gizmo")
        {
            
        }
    }

    public void PlayLapMade()
    {
        if(characterName == "morgan")
        {
            
        }
        else if(characterName == "emma")
        {
            
        }
        else if(characterName == "kai")
        {
            
        }
        else if(characterName == "reese")
        {
            
        }
        else if(characterName == "jamster")
        {
            
        }
        else if(characterName == "gizmo")
        {
            
        }
    }

    public void PlayRacerPassed()
    {
        if(characterName == "morgan")
        {
            
        }
        else if(characterName == "emma")
        {
            
        }
        else if(characterName == "kai")
        {
            
        }
        else if(characterName == "reese")
        {
            
        }
        else if(characterName == "jamster")
        {
            
        }
        else if(characterName == "gizmo")
        {
            
        }
    }

    public void PlayGetHit()
    {
        if(characterName == "morgan")
        {
            
        }
        else if(characterName == "emma")
        {
            
        }
        else if(characterName == "kai")
        {
            
        }
        else if(characterName == "reese")
        {
            
        }
        else if(characterName == "jamster")
        {
            
        }
        else if(characterName == "gizmo")
        {
            
        }
    }
}
