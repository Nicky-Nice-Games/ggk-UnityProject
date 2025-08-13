using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using static Unity.Services.Lobbies.Models.QueryFilter;

public class GrandPrixSelectHandler : MonoBehaviour
{
    [SerializeField] private List<GameObject> grandPrixOptions = new List<GameObject>();
    private GameManager gamemanagerObj;

    [SerializeField] private Button grandPrix1;

    // Start is called before the first frame update
    void Start()
    {
        gamemanagerObj = FindAnyObjectByType<GameManager>();

        foreach (GameObject obj in grandPrixOptions)
        {
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            gamemanagerObj.GetComponent<ButtonBehavior>().OnClick());

            button.onClick.AddListener(() => DisableButtons());
        }

        // Grand Prix 1 maps
        grandPrix1.onClick.AddListener(() =>
        gamemanagerObj.GrandPrixSelected(new List<string> {"LD_RITOuterLoop", "LD_RITDorm", "GSP_Golisano", "GSP_FinalsBrickRoad"}));

        if(MusicStateManager.instance != null)
        {
            MusicLapStateManager.instance.SetLapState(LapState.None);
            MusicResultsStateManager.instance.SetResultsState(ResultsState.None);
            MusicStateManager.instance.SetMusicState(MusicState.Menu);
        }
    }

    private void DisableButtons()
    {
        foreach (GameObject obj in grandPrixOptions)
        {
            obj.GetComponent<Button>().enabled = false;
        }
    }
}
