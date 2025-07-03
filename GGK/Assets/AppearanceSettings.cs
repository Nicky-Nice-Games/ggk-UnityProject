using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppearanceSettings : MonoBehaviour
{
    public Sprite icon;
    public Color color;
    public string name;
    public List<GameObject> models; // List of GameObjects representing different character models
    CharacterData characterData; // Reference to CharacterData script
    // Start is called before the first frame update
    void Start()
    {
        characterData = FindAnyObjectByType<CharacterData>();
        name = characterData.characterName;
        if(characterData != null)
        {
            for (int i = 0; i < models.Count; i++)
                    {
                        //Setting active correct model
                        if(name == models[i].name)
                        {
                            models[i].SetActive(true);
                            break;
                        }
            
                        //deleting the models that are not supposed to be active
                        Destroy(models[i]);
            

                    }
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        //UPDATE ME LATER

        
    }
}
