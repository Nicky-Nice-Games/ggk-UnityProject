using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

/// <summary>
/// Converts primitive data into character information.
/// By Keelon
/// </summary>
public static class CharacterBuilder
{
    public enum Character
    {
        gizmo = 0,
        morgan = 1,
        reese = 2,
        emma = 3,
        kai = 4,
        jamster = 5,
    }

    private static int characterCount;

    private static int lastPlayerAdded;
    private static List<Sprite> icons;

    private static List<Color> colors;

    //the loadout of NPC characters
    private static List<KeyValuePair<Character, Color>> characterLoadout;

    //a dictionary of the amount of times a character appears in the loadout
    private static Dictionary<Character, int> characterInstances;

    static CharacterBuilder()
    {
        characterCount = Enum.GetNames(typeof(Character)).Length;

        lastPlayerAdded = UnityEngine.Random.Range(0, characterCount - 1);

        icons = new List<Sprite>() {
            Resources.Load<Sprite>("Characters/MapIcons/gizmo"),
            Resources.Load<Sprite>("Characters/MapIcons/morgan"),
            Resources.Load<Sprite>("Characters/MapIcons/reese"),
            Resources.Load<Sprite>("Characters/MapIcons/emma"),
            Resources.Load<Sprite>("Characters/MapIcons/kai"),
            Resources.Load<Sprite>("Characters/MapIcons/jamster")
        };

        colors = new List<Color>()
        {
            //black
            new Color(0.1568628f, 0.1568628f, 0.1568628f),
            //gray
            new Color(0.6784314f, 0.6392157f, 6039216f),
            //greenish
            new Color(0.5176471f, 0.7411765f, 0),
            //orange
            new Color(0.9686275f, 0.4117647f, 0.01568628f),
            //tealish-blue
            new Color(0, 0.6117647f, 0.7411765f),
            //poiple
            new Color(0.4941177f, 0.3333333f, 0.7764707f),
            //red
            new Color(0.854902f, 0.1568628f, 0.1098039f)
        };

        characterLoadout = new List<KeyValuePair<Character, Color>>();
        characterInstances = new Dictionary<Character, int>();
    }

    //------------------------------------------------------------------------------------------
    // DATA TRANSlATION
    //------------------------------------------------------------------------------------------
    public static Sprite EnumToSprite(Character icon)
    {
        if ((int)icon < 0 || (int)icon > (icons.Count - 1)) return icons[0];
        return icons[(int)icon];
    }

    public static Sprite NameToSprite(string name)
    {
        if (name == null) name = "gizmo";
        Character choice;
        //if character's name or model name is an existing one
        if (Enum.TryParse(name, out choice) || Enum.TryParse(ModelToName(name), out choice))
        {
            return icons[(int)choice];
        }
        return icons[0];
    }

    public static string NameToModel(string name)
    {
        switch (name)
        {
            case "morgan":
                return "skater";
            case "reese":
                return "dining";
            case "emma":
                return "ol";
            case "kai":
                return "hockey";
            case "jamster":
                return name;
            default:
                return "gizmo";
        }
    }

    public static string ModelToName(string model)
    {
        switch (model)
        {
            case "skater":
                return "morgan";
            case "dining":
                return "reese";
            case "ol":
                return "emma";
            case "hockey":
                return "kai";
            case "jamster":
                return model;
            default:
                return "gizmo";
        }
    }
    

    //------------------------------------------------------------------------------------------
    // CHARACTER CREATION
    //------------------------------------------------------------------------------------------

    /// <summary>
    /// Creates a Key-value pair representing a new randomly-generated chartacter.
    /// </summary>
    private static KeyValuePair<Character, Color> CreateNewCharacter()
    {
        //generates an integer representing a random character enum
        int i = UnityEngine.Random.Range(0, characterCount - 1);
        int c = UnityEngine.Random.Range(0, colors.Count - 1);
        //                                         v character enum   v random color
        return new KeyValuePair<Character, Color>((Character)i, colors[c]);
    }

    /// <summary>
    /// Creates a Key-value pair representing a new randomly-generated chartacter.
    /// </summary>
    /// <param name="character">A pre-set character.</param>
    /// <returns></returns>
    private static KeyValuePair<Character, Color> CycleNewCharacter()
    {
        lastPlayerAdded = (lastPlayerAdded + 1) % characterCount;
        int c = UnityEngine.Random.Range(0, colors.Count - 1);
        //                                         v character enum   v random color
        return new KeyValuePair<Character, Color>((Character)lastPlayerAdded, colors[c]);
        
    }

    /// <summary>
    /// Generates character data for a random character.
    /// </summary>
    /// <param name="settings">The AppearanceSettings object the data should be filed into</param>
    public static void RandomizeAppearance(AppearanceSettings settings)
    {
        KeyValuePair<Character, Color> character = CreateNewCharacter();

        string charName = Enum.GetName(typeof(Character), character.Key);
        settings.gameObject.name = charName;
        settings.kartName = charName;
        settings.icon = EnumToSprite(character.Key);
        settings.color = character.Value;
        settings.UpdateModel();
        AddCharacter(character);
    }
    /// <summary>
    /// Generates character data for a random character, taking into account characters that have been generated before
    /// </summary>
    /// <param name="settings">The AppearanceSettings object the data should be filed into</param>
    public static void RandomizeUniqueAppearance(AppearanceSettings settings)
    {
        KeyValuePair<Character, Color> character = CycleNewCharacter();
        List<Color> takenColors = GetAllColorsOfCharacter(character.Key);
        int loopcount = 0;
        //cycle to the next color if the character is taken
        while (takenColors.Contains(character.Value) && loopcount < colors.Count)
        {
            character = new KeyValuePair<Character, Color>(character.Key, colors[(colors.IndexOf(character.Value) + 1) % colors.Count]);
            loopcount++;

            if (loopcount >= colors.Count)
            {
                Debug.Log("hello");
            }
        }

        string charName = Enum.GetName(typeof(Character), character.Key);
        settings.gameObject.name = charName;
        settings.kartName = charName;
        settings.icon = EnumToSprite(character.Key);
        settings.color = character.Value;
        settings.UpdateModel();
        AddCharacter(character);

    }

    //------------------------------------------------------------------------------------------
    // These methods: im here too!
    //------------------------------------------------------------------------------------------

    /// <summary>
    /// Refreshes the character loadout, so a new batch can be created
    /// </summary>
    public static void StartCharacterBatch()
    {
        characterLoadout.Clear();
        lastPlayerAdded = UnityEngine.Random.Range(0, characterCount - 1);
    }

    public static void AddCharacter(KeyValuePair<Character, Color> character)
    {
        characterLoadout.Add(character);
        AddToInstances(character);
    }

    //generates a log of the amount of times each character appears in the character loadout
    public static void AddToInstances(KeyValuePair<Character, Color> character)
    {
        if (characterInstances.Keys.Contains(character.Key))
        {
            characterInstances[character.Key] += 1;
        }
        else
        {
            characterInstances.Add(character.Key, 1);
        }
    }

    //get all currently occupied colors taken by a character
    private static List<Color> GetAllColorsOfCharacter(Character c)
    {
        List<Color> log = new List<Color>();
        for (int i = 0; i < characterLoadout.Count; i++)
        {
            if (characterLoadout[i].Key == c && !log.Contains(characterLoadout[i].Value))
            {
                log.Add(characterLoadout[i].Value);
            }
        }

        return log;
    }

}
