using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Triggers the Wwise sound events for the announcer lines.
/// </summary>
public class AnnouncerLines : MonoBehaviour
{
    //These are all the Wwise Events for the announcer.
    [Header("Wwise Sound Events")]
    [SerializeField] AK.Wwise.Event AllNighterExpressway;
    [SerializeField] AK.Wwise.Event CampusCircuit;
    [SerializeField] AK.Wwise.Event DormRoomDerby;
    [SerializeField] AK.Wwise.Event Emma;
    [SerializeField] AK.Wwise.Event Gizmo;
    [SerializeField] AK.Wwise.Event GrandPrix;
    [SerializeField] AK.Wwise.Event Jamster;
    [SerializeField] AK.Wwise.Event Kai;
    [SerializeField] AK.Wwise.Event Morgan;
    [SerializeField] AK.Wwise.Event Multiplayer;
    [SerializeField] AK.Wwise.Event QuickRace;
    [SerializeField] AK.Wwise.Event Race;
    [SerializeField] AK.Wwise.Event Reese;
    [SerializeField] AK.Wwise.Event Singleplayer;
    [SerializeField] AK.Wwise.Event TechHouseTurnpike;
    [SerializeField] AK.Wwise.Event TimeTrial;

    //These scripts play each of the Wwise sound events for the announcer lines.
    void PlayAllNighterExpressway()
    {
        AllNighterExpressway.Post(gameObject);
    }

    void PlayCampusCircuit()
    {
        CampusCircuit.Post(gameObject);
    }

    void PlayDormRoomDerby()
    {
        DormRoomDerby.Post(gameObject);
    }

    void PlayEmma()
    {
        Emma.Post(gameObject);
    }

    void PlayGizmo()
    {
        Gizmo.Post(gameObject);
    }

    void PlayGrandPrix()
    {
        GrandPrix.Post(gameObject);
    }

    void PlayJamster()
    {
        Jamster.Post(gameObject);
    }

    void PlayKai()
    {
        Kai.Post(gameObject);
    }

    void PlayMorgan()
    {
        Morgan.Post(gameObject);
    }

    void PlayMultiplayer()
    {
        Multiplayer.Post(gameObject);
    }

    void PlayQuickRace()
    {
        QuickRace.Post(gameObject);
    }

    void PlayRace()
    {
        Race.Post(gameObject);
    }

    void PlayReese()
    {
        Reese.Post(gameObject);
    }

    void PlaySingleplayer()
    {
        Singleplayer.Post(gameObject);
    }

    void PlayTechHouseTurnpike()
    {
        TechHouseTurnpike.Post(gameObject);
    }

    void PlayTimeTrial()
    {
        TimeTrial.Post(gameObject);
    }
}
