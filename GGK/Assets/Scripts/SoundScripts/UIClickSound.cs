using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIClickSound : MonoBehaviour
{
    
    public void onClickConfirm()
    {
        AkUnitySoundEngine.PostEvent("Play_select_confirm", gameObject);
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
