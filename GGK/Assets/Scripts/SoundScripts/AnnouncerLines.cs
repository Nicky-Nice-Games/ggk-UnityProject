using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Triggers the Wwise sound events for the announcer lines.
/// </summary>
public class AnnouncerLines : MonoBehaviour
{
    public static AnnouncerLines instance;

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

    void Awake()
    {
        // only have one instance of this script at a time
        if (instance == null)
        {
            instance = this;

            // persists across scenes
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // destroy this script if one already exists
            Destroy(gameObject);
        }
    }

    //These scripts play each of the Wwise sound events for the announcer lines.
    public void PlayAllNighterExpressway()
    {
        AllNighterExpressway.Post(gameObject);
    }

    public void PlayCampusCircuit()
    {
        CampusCircuit.Post(gameObject);
    }

    public void PlayDormRoomDerby()
    {
        DormRoomDerby.Post(gameObject);
    }

    public void PlayEmma()
    {
        Emma.Post(gameObject);
    }

    public void PlayGizmo()
    {
        Gizmo.Post(gameObject);
    }

    public void PlayGrandPrix()
    {
        GrandPrix.Post(gameObject);
    }

    public void PlayJamster()
    {
        Jamster.Post(gameObject);
    }

    public void PlayKai()
    {
        Kai.Post(gameObject);
    }

    public void PlayMorgan()
    {
        Morgan.Post(gameObject);
    }

    public void PlayMultiplayer()
    {
        Multiplayer.Post(gameObject);
    }

    public void PlayQuickRace()
    {
        QuickRace.Post(gameObject);
    }

    public void PlayRace()
    {
        Race.Post(gameObject);
    }

    public void PlayReese()
    {
        Reese.Post(gameObject);
    }

    public void PlaySingleplayer()
    {
        Singleplayer.Post(gameObject);
    }

    public void PlayTechHouseTurnpike()
    {
        TechHouseTurnpike.Post(gameObject);
    }

    public void PlayTimeTrial()
    { 
        TimeTrial.Post(gameObject);
    }
}
