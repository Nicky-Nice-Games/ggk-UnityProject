using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIClickSound : MonoBehaviour
{

    public void onClickConfirm()
    {
        AkSoundEngine.PostEvent("Play_select_confirm", gameObject);
    }

    public void onClickBack()
    {
        AkSoundEngine.PostEvent("Play_select_back_item_drop", gameObject);

    }

    public void onClickStart()
    {
        AkSoundEngine.PostEvent("Play_select_start", gameObject);

    }
}
