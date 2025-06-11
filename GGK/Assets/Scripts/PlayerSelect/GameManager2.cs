using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager2 : MonoBehaviour
{
    [SerializeField]
    List<GameObject> racers;
    string playerChoice;
    GameObject racer;
    Renderer racerRenderer;
    // Start is called before the first frame update
    void Start()
    {
        playerChoice = RacerDataManager.instance.RacerChoice;
        switch (playerChoice)
        {
            case "Square":
                racer = Instantiate(racers[0], new Vector3(0, 0, 0), Quaternion.identity);
                break;
            case "Sphere":
                racer = Instantiate(racers[1], new Vector3(0, 0, 0), Quaternion.identity);
                break;
            case "Capsule":
                racer = Instantiate(racers[2], new Vector3(0, 0, 0), Quaternion.identity);
                break;
        }
        racerRenderer = racer.GetComponent<Renderer>();
        racerRenderer.material = RacerDataManager.instance.ColorChoice;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
