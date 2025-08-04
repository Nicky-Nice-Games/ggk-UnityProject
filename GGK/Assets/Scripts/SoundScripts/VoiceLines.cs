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
    [Header("Emma")]
    [SerializeField] AK.Wwise.Event EmmaHitCollision;
    [SerializeField] AK.Wwise.Event EmmaItemThrown;
    [SerializeField] AK.Wwise.Event EmmaLapMade;
    [SerializeField] AK.Wwise.Event EmmaRacerPassed;

    [Header("Gizmo")]
    [SerializeField] AK.Wwise.Event GizmoHitCollision;
    [SerializeField] AK.Wwise.Event GizmoItemThrown;
    [SerializeField] AK.Wwise.Event GizmoLapMade;
    [SerializeField] AK.Wwise.Event GizmoRacerPassed;

    [Header("Jamster")]
    [SerializeField] AK.Wwise.Event JamsterHitCollision;
    [SerializeField] AK.Wwise.Event JamsterItemThrown;
    [SerializeField] AK.Wwise.Event JamsterLapMade;
    [SerializeField] AK.Wwise.Event JamsterRacerPassed;

    [Header("Kai")]
    [SerializeField] AK.Wwise.Event KaiHitCollision;
    [SerializeField] AK.Wwise.Event KaiItemThrown;
    [SerializeField] AK.Wwise.Event KaiLapMade;
    [SerializeField] AK.Wwise.Event KaiRacerPassed;

    [Header("Morgan")]
    [SerializeField] AK.Wwise.Event MorganHitCollision;
    [SerializeField] AK.Wwise.Event MorganItemThrown;
    [SerializeField] AK.Wwise.Event MorganLapMade;
    [SerializeField] AK.Wwise.Event MorganRacerPassed;

    [Header("Reese")]
    [SerializeField] AK.Wwise.Event ReeseHitCollision;
    [SerializeField] AK.Wwise.Event ReeseItemThrown;
    [SerializeField] AK.Wwise.Event ReeseLapMade;
    [SerializeField] AK.Wwise.Event ReeseRacerPassed;

    // Start is called before the first frame update
    void Start()
    {
        //The name of the current character is retrieved
        //from AppearanceSettings on the current GameObject.
        characterName = GetComponent<AppearanceSettings>().name;
    }

    public void PlayHitCollision()
    {
        //Depending on what the name of the character is, the voice lines used are different.
        //Thus, the specific event for that character and this circumstance is called.
        //Same structure with the following methods.
        if (characterName == "emma")
        {
            EmmaHitCollision.Post(gameObject);
        }
        else if (characterName == "gizmo")
        {
            GizmoHitCollision.Post(gameObject);
        }
        else if (characterName == "jamster")
        {
            JamsterHitCollision.Post(gameObject);
        }
        else if (characterName == "kai")
        {
            KaiHitCollision.Post(gameObject);
        }
        else if (characterName == "morgan")
        {
            MorganHitCollision.Post(gameObject);
        }
        else if (characterName == "reese")
        {
            ReeseHitCollision.Post(gameObject);
        }
    }

    public void PlayItemThrown()
    {
        if (characterName == "emma")
        {
            EmmaItemThrown.Post(gameObject);
        }
        else if (characterName == "gizmo")
        {
            GizmoItemThrown.Post(gameObject);
        }
        else if (characterName == "jamster")
        {
            JamsterItemThrown.Post(gameObject);
        }
        else if (characterName == "kai")
        {
            KaiItemThrown.Post(gameObject);
        }
        else if (characterName == "morgan")
        {
            MorganItemThrown.Post(gameObject);
        }
        else if (characterName == "reese")
        {
            ReeseItemThrown.Post(gameObject);
        }
    }

    public void PlayLapMade()
    {
        if (characterName == "emma")
        {
            EmmaLapMade.Post(gameObject);
        }
        else if (characterName == "gizmo")
        {
            GizmoLapMade.Post(gameObject);
        }
        else if (characterName == "jamster")
        {
            JamsterLapMade.Post(gameObject);
        }
        else if (characterName == "kai")
        {
            KaiLapMade.Post(gameObject);
        }
        else if (characterName == "morgan")
        {
            MorganLapMade.Post(gameObject);
        }
        else if (characterName == "reese")
        {
            ReeseLapMade.Post(gameObject);
        }
    }

    public void PlayRacerPassed()
    {
        if (characterName == "emma")
        {
            EmmaRacerPassed.Post(gameObject);
        }
        else if (characterName == "gizmo")
        {
            GizmoRacerPassed.Post(gameObject);
        }
        else if (characterName == "jamster")
        {
            JamsterRacerPassed.Post(gameObject);
        }
        else if (characterName == "kai")
        {
            KaiRacerPassed.Post(gameObject);
        }
        else if (characterName == "morgan")
        {
            MorganRacerPassed.Post(gameObject);
        }
        else if (characterName == "reese")
        {
            ReeseRacerPassed.Post(gameObject);
        }
    }
}