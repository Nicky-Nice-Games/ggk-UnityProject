using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    public enum MusicState
    {
        Dorm,
        FBR,
        Menu,
        None,
        OuterLoop
    }
    public MusicState musicState;
}
