using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIClickSound : MonoBehaviour
{
    public static UIClickSound instance;

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

    public void onClickConfirm()
    {
        Debug.Log(AkUnitySoundEngine.PostEvent("Play_select_confirm", gameObject));
    }

    public void onClickBack()
    {
        AkUnitySoundEngine.PostEvent("Play_select_back_item_drop", gameObject);

    }

    public void onClickStart()
    {
        AkUnitySoundEngine.PostEvent("Play_select_start", gameObject);

    }
}
