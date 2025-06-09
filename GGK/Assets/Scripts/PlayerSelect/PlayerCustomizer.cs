using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCustomizer : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    List<GameObject> racers;

    [SerializeField]
    List<Material> colors;
    int currentColor;

    GameObject racer;
    Renderer racerRenderer;

    private string playerChoice;
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
        currentColor = 0;
        racerRenderer.material = colors[currentColor];
        RacerDataManager.instance.ColorChoice = racerRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateSelection(int i)
    {
        if (currentColor == colors.Count - 1 && i == 1)
        {
            currentColor = 0;
        }
        else if (currentColor == 0 && i == -1)
        {
            currentColor = colors.Count - 1;
        }
        else
        {
            currentColor += i;
        }
        racerRenderer.material = colors[currentColor];
        RacerDataManager.instance.ColorChoice = racerRenderer.material;
    }

    public void MoveRight()
    {
        UpdateSelection(1);
    }

    public void MoveLeft()
    {
        UpdateSelection(-1);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameCustomScene");
    }
}
