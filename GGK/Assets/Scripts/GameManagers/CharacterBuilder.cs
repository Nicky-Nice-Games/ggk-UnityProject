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
    private static int lastColorAdded;
    private static List<Sprite> icons;

    private static List<Color> colors;

    //the loadout of characters
    private static List<KeyValuePair<Character, Color>> characterLoadout;

    //a dictionary of the amount of times a character appears in the loadout
    private static Dictionary<Character, int> characterInstances;

    static CharacterBuilder()
    {
        characterCount = Enum.GetNames(typeof(Character)).Length;

        

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
            //maroon
            new Color(0.482353f, 0.09019608f, 0.1568628f),
            //red
            new Color(0.8392158f, 0.2117647f, 0.145098f),
            //orange
            new Color(0.9686275f, 0.3960785f, 0.1333333f),
            //yeller
            new Color(0.9058824f, 0.937255f, 0.2f),
            //green
            new Color(0.1058824f, 0.8431373f, 0.3686275f),
            //cyan
            new Color(0.1137255f, 0.8901961f, 0.8941177f),
            //blue
            new Color(0f, 0.4196079f, 0.7960785f),
            //indigo
            new Color(0.1882353f, 0.1764706f, 0.5333334f),
            //poiple
            new Color(0.4745098f, 0.1882353f, 0.654902f),
            //magenta
            new Color(0.8705883f, 0.1764706f, 0.482353f),
            //ivory
            new Color(0.9843138f, 0.9686275f, 0.937255f),
            //midnighttuh
            new Color(0.09019608f, 0.07843138f, 0.1058824f),
        };
        lastPlayerAdded = UnityEngine.Random.Range(0, characterCount - 1);
        lastColorAdded = UnityEngine.Random.Range(0, colors.Count - 1);

        characterLoadout = new List<KeyValuePair<Character, Color>>();
        characterInstances = new Dictionary<Character, int>();
    }

    //------------------------------------------------------------------------------------------
    // DATA TRANSlATION
    //------------------------------------------------------------------------------------------

    //returns a character's sprite from their character enum.
    public static Sprite EnumToSprite(Character icon)
    {
        if ((int)icon < 0 || (int)icon > (icons.Count - 1)) return icons[0];
        return icons[(int)icon];
    }

    /// <summary>
    /// Returns a character's sprite from their name.
    /// </summary>
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

    //DEPRECATED--NPCS USE UPDATED MODELS
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

    /// <summary>
    /// Translates a character's beta model name into the updated model name.
    /// </summary>
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
    private static KeyValuePair<Character, Color> CycleNewCharacter()
    {
        //cycles characters through to the next character and next color, looping back around if it reaches the max possible index 
        lastPlayerAdded = (lastPlayerAdded + 1) % characterCount;
        lastColorAdded = (lastColorAdded + 1) % colors.Count;
        int loopcount = 0;
        KeyValuePair<Character, Color> newChar;
        do
        {
            //                                                v character enum             v random color
            newChar = new KeyValuePair<Character, Color>((Character)lastPlayerAdded, colors[lastColorAdded]);
            loopcount++;

            if (loopcount >= characterCount)
            {
                Debug.Log("Possible infinite loop detected in CharacterBuilder--breaking while-loop early");
            }
        }
        while (loopcount >= characterCount && characterInstances.ContainsKey(newChar.Key) && characterInstances[newChar.Key] > 1);
        //                                       ^change this to not hardcode later 

        return newChar;
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
                Debug.Log("Possible infinite loop detected in CharacterBuilder--breaking while-loop early");
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
        characterInstances.Clear();
        lastPlayerAdded = UnityEngine.Random.Range(0, characterCount - 1);
    }

    /// <summary>
    /// Adds a character to the character loadout.
    /// </summary>
    public static void AddCharacter(KeyValuePair<Character, Color> character)
    {
        characterLoadout.Add(character);
        AddToInstances(character);
    }

    public static void AddCharacter(AppearanceSettings settings)
    {
        Debug.Log(settings.name);
        KeyValuePair<Character, Color> character = new KeyValuePair<Character, Color>((Character)Enum.Parse(typeof(Character), settings.name), settings.color);
        AddCharacter(character);
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
