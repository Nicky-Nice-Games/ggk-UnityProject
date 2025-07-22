using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    private GameManager gameManager;


    private void Start()
    {
        gameManager = GetComponent<GameManager>();
    }

    /// <summary>
    /// Setting up the post request
    /// </summary>
    /// <param name="url">where to send the data</param>
    /// <param name="jsonData">json data</param>
    /// <returns></returns>
    public async Task PostJsonAsync(string url, string json)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            var operation = webRequest.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("POST ERROR: " + webRequest.error);
            }
            else
            {
                Debug.Log("POST Success");
            }
        }
    }

    /// <summary>
    /// Starts the corutine for sending json data
    /// </summary>
    /// <param name="thisPlayer">the player whos data will be set</param>
    /// <returns></returns>
    public void PostPlayerData(PlayerInfo thisPlayer)
    {
        // Serializing data to send back
        SerializablePlayerInfo serializable = gameObject.GetComponent<SerializablePlayerInfo>();
        serializable.ConvertToSerializable(thisPlayer);
        string json = JsonUtility.ToJson(serializable);

        //StartCoroutine(PostJson("https://maventest-a9cc74b8d5cf.herokuapp.com/gameservice/gamelog", json));
    }

    /// <summary>
    /// Starts the corutine for getting json data
    /// </summary>
    /// <param name="pid">the player ID</param>
    /// <param name="thisPlayer">the player whos data will be set</param>
    /// <returns></returns>
    private IEnumerator GetPlayerWithPid(string pid, PlayerInfo thisPlayer)
    {
        // Sending the request for the existing player info (gameservice/gamelog/player/{pid})
        string getPath = "https://maventest-a9cc74b8d5cf.herokuapp.com/gameservice/gamelog/player/" + pid;
        Debug.Log("Path: " + getPath);

        using (UnityWebRequest webRequest = UnityWebRequest.Get(getPath))
        {
            // Requesting and waiting for the desired data
            yield return webRequest.SendWebRequest();

            // Checking good request
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // Getting the data from the json get request and setting the player data
                string data = webRequest.downloadHandler.text;
                PlayerInfo userData = JsonUtility.FromJson<PlayerInfo>(data);
                thisPlayer = userData;
                Debug.Log($"Updated player data!\n{thisPlayer}");
            }
            else
            {
                Debug.Log("Failed to get data" + webRequest.error);
            }
        }
    }

    public IEnumerator GetPlayerWithNamePass(string username, string password, PlayerInfo thisPlayer)
    {
        // Sending the request for the existing player info (gameservice/playerlog/login/{username}/{password})
        string getPath = "https://maventest-a9cc74b8d5cf.herokuapp.com/gameservice/playerlog/login/" + username + "/" + password;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(getPath))
        {
            // Requesting and waiting for the desired data
            yield return webRequest.SendWebRequest();

            // Checking good request
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Found player");

                // Gets the basic user data that holds pid then using that to get the rest of players data
                string data = webRequest.downloadHandler.text;
                WebUserData userData = JsonUtility.FromJson<WebUserData>(data);
                GetPlayerWithPid(userData.pid, thisPlayer);
            }
            else
            {
                Debug.Log("Failed to get data" + webRequest.error);
            }
        }
    }

    /// <summary>
    /// Creates a new user in the database 
    /// </summary>
    /// <param name="thisPlayer"></param>
    /// <returns>True if player was created</returns>
    public async Task<bool> CreatePlayerAsync(PlayerInfo thisPlayer)
    {
        WebUserData webUserData = new WebUserData
        {
            username = thisPlayer.playerName,
            password = thisPlayer.playerPassword,
            email = thisPlayer.playerEmail
        };

        string path = "https://maventest-a9cc74b8d5cf.herokuapp.com/gameservice/playerlog/create";
        string json = JsonUtility.ToJson(webUserData);

        bool playerExists = await CheckForPlayerInDataAsync(webUserData.email);
        Debug.Log("Player found in server?: " + playerExists);

        if (!playerExists)
        {
            await PostJsonAsync(path, json);
            Debug.Log("Player created");
            return true;
        }

        Debug.Log("Player already exists");
        return false;
    }

    /// <summary>
    /// Using email to check if player exists in online database
    /// </summary>
    /// <param name="email">email</param>
    /// <param name="callback">the bool that will be returned as a callback</param>
    /// <returns></returns>
    public async Task<bool> CheckForPlayerInDataAsync(string email)
    {
        string url = "https://maventest-a9cc74b8d5cf.herokuapp.com/gameservice/playerlog/" + email;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield(); // Let Unity update frame

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string json = webRequest.downloadHandler.text;
                return bool.Parse(json.ToLower());
            }
            else
            {
                Debug.Log("GET ERROR FOR PLAYER CHECK: " + webRequest.error);
                return false;
            }
        }
    }

    // Wrapper to call async from non-async
    public void CreatePlayer(PlayerInfo thisPlayer)
    {
        _ = CallAsyncFromNonAsync(thisPlayer);
    }

    private async Task CallAsyncFromNonAsync(PlayerInfo thisPlayer)
    {
        bool wasCreated = await CreatePlayerAsync(thisPlayer);
        Debug.Log("Was created (async): " + wasCreated);

        if (!wasCreated)
        {
            VirtualKeyboardController kbController = FindAnyObjectByType<VirtualKeyboardController>();
            kbController.ResetCurrentFields();
            return;
        }
        else
        {
            gameManager.LoggedIn();
        }
        
    }
}