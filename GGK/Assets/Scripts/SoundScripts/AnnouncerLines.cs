using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Triggers the Wwise sound events for the announcer lines.
/// </summary>
public class AnnouncerLines : MonoBehaviour
{
    //These are all the Events planned for the announcer.
    [Header("Wwise Sound Events")]
    [SerializeField] AK.Wwise.Event SinglePlayer;
    [SerializeField] AK.Wwise.Event MultiPlayer;
    [SerializeField] AK.Wwise.Event RaceGameMode;
    [SerializeField] AK.Wwise.Event GrandPrixGameMode;
    [SerializeField] AK.Wwise.Event TimeTrialGameMode;
    [SerializeField] AK.Wwise.Event MorganSelect;
    [SerializeField] AK.Wwise.Event ReeseSelect;
    [SerializeField] AK.Wwise.Event KaiSelect;
    [SerializeField] AK.Wwise.Event EmmaSelect;
    [SerializeField] AK.Wwise.Event GizmoSelect;
    [SerializeField] AK.Wwise.Event JamsterSelect;
    [SerializeField] AK.Wwise.Event CampusCircuitMap;
    [SerializeField] AK.Wwise.Event TechHouseTurnpikeMap;
    [SerializeField] AK.Wwise.Event DormRoomDerbyMap;
    [SerializeField] AK.Wwise.Event AllNighterExpresswayMap;

    void PlaySinglePlayer()
    {
        SinglePlayer.Post(gameObject);
    }

    void PlayMultiPlayer()
    {
        MultiPlayer.Post(gameObject);
    }

    void PlayRaceGameMode()
    {
        RaceGameMode.Post(gameObject);
    }

    void PlayGrandPrixGameMode()
    {
        GrandPrixGameMode.Post(gameObject);
    }

    void PlayTimeTrialGameMode()
    {
        TimeTrialGameMode.Post(gameObject);
    }

    void PlayMorganSelect()
    {
        MorganSelect.Post(gameObject);
    }

    void PlayReeseSelect()
    {
        ReeseSelect.Post(gameObject);
    }

    void PlayKaiSelect()
    {
        KaiSelect.Post(gameObject);
    }

    void PlayEmmaSelect()
    {
        EmmaSelect.Post(gameObject);
    }

    void PlayGizmoSelect()
    {
        GizmoSelect.Post(gameObject);
    }

    void PlayJamsterSelect()
    {
        JamsterSelect.Post(gameObject);
    }

    void PlayCampusCircuitMap()
    {
        CampusCircuitMap.Post(gameObject);
    }

    void PlayTechHouseTurnpikeMap()
    {
        TechHouseTurnpikeMap.Post(gameObject);
    }

    void PlayDormRoomDerbyMap()
    {
        DormRoomDerbyMap.Post(gameObject);
    }

    void PlayAllNighterExpresswayMap()
    {
        AllNighterExpresswayMap.Post(gameObject);
    }
}
