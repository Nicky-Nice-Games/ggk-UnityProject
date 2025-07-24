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

    //These are a bunch of Wwise events to play all the voice lines.
    //There are 6 characters, and 4 types of lines each.
    [Header("Morgan")]
    [SerializeField] AK.Wwise.Event MorganItemUsed;
    [SerializeField] AK.Wwise.Event MorganLapMade;
    [SerializeField] AK.Wwise.Event MorganRacerPassed;
    [SerializeField] AK.Wwise.Event MorganGetHit;

    [Header("Emma")]
    [SerializeField] AK.Wwise.Event EmmaItemUsed;
    [SerializeField] AK.Wwise.Event EmmaLapMade;
    [SerializeField] AK.Wwise.Event EmmaRacerPassed;
    [SerializeField] AK.Wwise.Event EmmaGetHit;

    [Header("Kai")]
    [SerializeField] AK.Wwise.Event KaiItemUsed;
    [SerializeField] AK.Wwise.Event KaiLapMade;
    [SerializeField] AK.Wwise.Event KaiRacerPassed;
    [SerializeField] AK.Wwise.Event KaiGetHit;

    [Header("Reese")]
    [SerializeField] AK.Wwise.Event ReeseItemUsed;
    [SerializeField] AK.Wwise.Event ReeseLapMade;
    [SerializeField] AK.Wwise.Event ReeseRacerPassed;
    [SerializeField] AK.Wwise.Event ReeseGetHit;

    [Header("Jamster")]
    [SerializeField] AK.Wwise.Event JamsterItemUsed;
    [SerializeField] AK.Wwise.Event JamsterLapMade;
    [SerializeField] AK.Wwise.Event JamsterRacerPassed;
    [SerializeField] AK.Wwise.Event JamsterGetHit;

    [Header("Gizmo")]
    [SerializeField] AK.Wwise.Event GizmoItemUsed;
    [SerializeField] AK.Wwise.Event GizmoLapMade;
    [SerializeField] AK.Wwise.Event GizmoRacerPassed;
    [SerializeField] AK.Wwise.Event GizmoGetHit;

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
            MorganItemUsed.Post(gameObject);
        }
        else if(characterName == "emma")
        {
            EmmaItemUsed.Post(gameObject);
        }
        else if(characterName == "kai")
        {
            KaiItemUsed.Post(gameObject);
        }
        else if(characterName == "reese")
        {
            ReeseItemUsed.Post(gameObject);
        }
        else if(characterName == "jamster")
        {
            JamsterItemUsed.Post(gameObject);
        }
        else if(characterName == "gizmo")
        {
            GizmoItemUsed.Post(gameObject);
        }
    }

    public void PlayLapMade()
    {
        if(characterName == "morgan")
        {
            MorganLapMade.Post(gameObject);
        }
        else if(characterName == "emma")
        {
            EmmaLapMade.Post(gameObject);
        }
        else if(characterName == "kai")
        {
            KaiLapMade.Post(gameObject);
        }
        else if(characterName == "reese")
        {
            ReeseLapMade.Post(gameObject);
        }
        else if(characterName == "jamster")
        {
            JamsterLapMade.Post(gameObject);
        }
        else if(characterName == "gizmo")
        {
            GizmoLapMade.Post(gameObject);
        }
    }

    public void PlayRacerPassed()
    {
        if(characterName == "morgan")
        {
            MorganRacerPassed.Post(gameObject);
        }
        else if(characterName == "emma")
        {
            EmmaRacerPassed.Post(gameObject);
        }
        else if(characterName == "kai")
        {
            KaiRacerPassed.Post(gameObject);
        }
        else if(characterName == "reese")
        {
            ReeseRacerPassed.Post(gameObject);
        }
        else if(characterName == "jamster")
        {
            JamsterRacerPassed.Post(gameObject);
        }
        else if(characterName == "gizmo")
        {
            GizmoRacerPassed.Post(gameObject);
        }
    }

    public void PlayGetHit()
    {
        if(characterName == "morgan")
        {
            MorganGetHit.Post(gameObject);
        }
        else if(characterName == "emma")
        {
            EmmaGetHit.Post(gameObject);
        }
        else if(characterName == "kai")
        {
            KaiGetHit.Post(gameObject);
        }
        else if(characterName == "reese")
        {
            ReeseGetHit.Post(gameObject);
        }
        else if(characterName == "jamster")
        {
            JamsterGetHit.Post(gameObject);
        }
        else if(characterName == "gizmo")
        {
            GizmoGetHit.Post(gameObject);
        }
    }
}
