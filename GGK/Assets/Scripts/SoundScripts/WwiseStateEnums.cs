using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//These are a set of enums to manage the Wwise music states.

/// <summary>
/// Determines which music track should be playing.
/// </summary>
public enum MusicState
{
    Dorm,
    FBR,
    Golisano,
    Menu,
    None,
    OuterLoop,
    PostRace
}

/// <summary>
/// Determines what lap we are on, so the music can adapt to it.
/// </summary>
public enum LapState
{
    Lap1,
    Lap2,
    Lap3,
    None
}

/// <summary>
/// Determines whether we are starting a race, in a race, or if we
/// won or lost. The music adapts to this.
/// </summary>
public enum ResultsState
{
    InProgress,
    Loss,
    None,
    Starting,
    Win
}