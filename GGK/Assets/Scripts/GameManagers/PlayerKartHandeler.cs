using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerKartHandeler : MonoBehaviour
{
    // References
    public GameManager gameManager;
    public List<GameObject> playerOptions;

    public CharacterData characterData;

    public List<GameObject> characterModels;
    public TextMeshProUGUI characterName;

    public Image characterSelectedImage;
    private Image prevCharacterImageBorder;

    public GameObject kartModel;
    private int rotation = 140;

    //[SerializeField] private Transform playerSelectPanel;
    //[SerializeField] private Transform waitingScreen;

    // Reference Lists
    public List<GameObject> characterButtons;
    public List<GameObject> colorButtons;
    public List<Color> colors;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        characterData = FindAnyObjectByType<CharacterData>();
        //waitingScreen.gameObject.SetActive(false);
        //playerSelectPanel.gameObject.SetActive(true);

        // Connect character buttons to ChangeCharacter with appropriate arguments
        foreach (GameObject characterButton in characterButtons)
        {
            Image[] images = characterButton.GetComponentsInChildren<Image>();

            Button btn = characterButton.GetComponentInChildren<Button>();

            btn.onClick.AddListener(() => ChangeCharacter(btn.name));
        }

        // Invoke default selection
        characterButtons[0].GetComponentInChildren<Button>().onClick.Invoke();

        // Assigning the buttons their listeners
        foreach (GameObject obj in playerOptions)
        {
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            gameManager.GetComponent<ButtonBehavior>().OnClick());
            button.onClick.AddListener(() =>
            gameManager.GetComponentInChildren<GameManager>().PlayerSelected());
        }

        //// During multiplayer build multiplayer panel
        //if (MultiplayerManager.Instance.IsMultiplayer)
        //{
        //    MultiplayerManager.Instance.AddPlayerToPanelRpc();
        //}
    }

    public void SwitchToWaitingScreen()
    {
        //playerSelectPanel.gameObject.SetActive(false);
        //waitingScreen.gameObject.SetActive(true);
    }
    public void SwitchToPlayerSelectPanel()
    {
        //playerSelectPanel.gameObject.SetActive(true);
        //waitingScreen.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Select this character button
    public void ChangeCharacter(string name)
    {
        //// Reset previous character button's border color to gray
        //if (prevCharacterImageBorder != null)
        //{
        //    prevCharacterImageBorder.enabled = false;
        //}
        //if (characterSelectedImage != null)
        //{
        //    characterSelectedImage.color = Color.white;
        //}
        //
        //// Change selected character information to those on the button
        //characterSelectedImage = characterImage;
        ////ColorChange(); // Change color of selected character image
        //characterName.text = name;
        
        // Set active the correct character model
        foreach(GameObject characterModel in characterModels)
        {
            if (characterModel.name.ToLower() != name.ToLower())
            {
                characterModel.SetActive(false);
            }
            else
            {
                characterModel.SetActive(true);
            }
        }
        // Save selected button information to change back later and change border color to yellow
       //border.enabled = true;
       //prevCharacterImageBorder = border;

        //// Save information to characterData script if it exist
        //if (characterData != null)
        //{
        //    characterData.characterSprite = characterImage.sprite;
        //    characterData.characterName = name;
        //}
            
    }

    // Which direction for the color carousel?
    //public void LeftColor()
    //{
    //    // Create a new list that shifts everything to the Right
    //    // Moving the left color towards middle
    //    List<Color> temp = new List<Color>();
    //
    //    temp.Add(colors[colors.Count - 1]);
    //
    //    for (int i = 0; i < colors.Count - 1; i++)
    //    {
    //        temp.Add(colors[i]);
    //    }
    //
    //    // New list overwrites old list
    //    colors = temp;
    //
    //    // Call this to set color of the color carousel
    //    ColorChange();
    //}
    //public void RightColor()
    //{
    //    // Create a new list that shifts everything to the left
    //    // Moving the right color towards middle
    //    List<Color> temp = new List<Color>();
    //
    //    for (int i = 1; i < colorButtons.Count; i++)
    //    {
    //        temp.Add(colors[i]);
    //    }
    //    temp.Add(colors[0]);
    //
    //    // New list overwrites old list
    //    colors = temp;
    //
    //    // Call this to set color of the color carousel
    //    ColorChange();
    //}
    //public void ColorChange()
    //{
    //    // Color Carousel Update
    //    for (int i = 0; i < colorButtons.Count; i++)
    //    {
    //        Image[] images = colorButtons[i].GetComponentsInChildren<Image>();
    //
    //        // This should be the inner color
    //        images[1].color = colors[i];
    //    }
    //
    //    // Change sprite color (SHOULD THIS CHANGE THE COLOR OF THE SPRITE OR THE KART?)
    //    Color middleColor = colors[3];
    //    characterSelectedImage.color = middleColor;
    //
    //    /*
    //    foreach (GameObject characterButton in characterButtons)
    //    {
    //        Button button = characterButton.GetComponentInChildren<Button>();
    //        ColorBlock colorBlock = button.colors;
    //        colorBlock.selectedColor = middleColor;
    //        button.colors = colorBlock;
    //    }
    //    */
    //
    //    if (characterData != null)
    //        characterData.characterColor = middleColor;
    //}
}
