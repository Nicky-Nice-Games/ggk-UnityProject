// Script to handle api calls and basic logic that needs to be preformed within API async functions
// Written by Logan Larrondo

using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

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
    private async Task PostJsonAsync(string url, string json)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            UnityWebRequestAsyncOperation operation = webRequest.SendWebRequest();
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
    public async void PostPlayerData(PlayerInfo thisPlayer)
    {
        Debug.Log("Called PostPlayerData in APIManager");
        // Serializing data to send back
        SerializablePlayerInfo serializable = new SerializablePlayerInfo();
        serializable.ConvertToSerializable(thisPlayer);
        string json = JsonConvert.SerializeObject(serializable);
        Debug.Log("Json: " + json);

        await PostJsonAsync("https://maventest-a9cc74b8d5cf.herokuapp.com/gameservice/gamelog", json);
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

    private async Task<bool> GetPlayerWithNamePassAsync(string username, string password, PlayerInfo thisPlayer)
    {
        // Sending the request for the existing player info (gameservice/playerlog/login/{username}/{password})
        string getPath = "https://maventest-a9cc74b8d5cf.herokuapp.com/gameservice/playerlog/login/" + username + "/" + password;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(getPath))
        {
            // Requesting and waiting for the desired data
            UnityWebRequestAsyncOperation operation = webRequest.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            // Checking good request
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Found player");

                // Gets the basic user data that holds pid then using that to get the rest of players data
                string data = webRequest.downloadHandler.text;
                WebUserData userData = JsonUtility.FromJson<WebUserData>(data);
                Debug.Log("PlayerID: " + userData.pid);
                thisPlayer.pid = userData.pid;
                GetPlayerWithPid(userData.pid, thisPlayer);
                return true;
            }
            else
            {
                Debug.Log("Failed to get data" + webRequest.error);
                return false;
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

        bool playerExists = await CheckForPlayerInDataAsync(webUserData.email, webUserData.username);
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
    /// <returns>True if player is found</returns>
    private async Task<bool> CheckForPlayerInDataAsync(string email, string username)
    {
        bool emailAvailable = await VerifyEmailAsync(email);
        Debug.Log("Email is avalible: " + emailAvailable);
        bool usernameAvailable = await VerifyUsernameAsync(username);
        Debug.Log("Username is avalible: " + usernameAvailable);
        SignInManager signInManager = FindAnyObjectByType<SignInManager>();

        if (!emailAvailable && !usernameAvailable)
        {
            VirtualKeyboardController kbController = FindAnyObjectByType<VirtualKeyboardController>();
            kbController.ResetCurrentFields();
            signInManager.usernameError.SetActive(true);
            signInManager.emailError.SetActive(true);
            return true;
        }
        if (!emailAvailable)
        {
            VirtualKeyboardController kbController = FindAnyObjectByType<VirtualKeyboardController>();
            kbController.ResetCurrentFields();
            signInManager.usernameError.SetActive(false);
            signInManager.emailError.SetActive(true);
            return true;
        }
        if(!usernameAvailable)
        {
            VirtualKeyboardController kbController = FindAnyObjectByType<VirtualKeyboardController>();
            kbController.ResetCurrentFields();
            signInManager.usernameError.SetActive(true);
            signInManager.emailError.SetActive(false);
            return true;
        }
        signInManager.usernameError.SetActive(false);
        signInManager.emailError.SetActive(false);
        return false;
    }

    /// <summary>
    /// Async that will handle logic 
    /// </summary>
    /// <param name="thisPlayer"></param>
    /// <returns></returns>
    public async Task CreatePlayer(PlayerInfo thisPlayer)
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

    /// <summary>
    /// Async that will handle logic
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task CheckPlayer(PlayerInfo playerInfo)
    {
        SignInManager signInManager = FindAnyObjectByType<SignInManager>();
        bool wasFound = await GetPlayerWithNamePassAsync(playerInfo.playerName, playerInfo.playerPassword, playerInfo);
        Debug.Log("Was found (async): " + wasFound);

        if (!wasFound)
        {
            VirtualKeyboardController kbController = FindAnyObjectByType<VirtualKeyboardController>();
            kbController.ResetCurrentFields();
            signInManager.loginError.SetActive(true);
            return;
        }
        else
        {
            signInManager.loginError.SetActive(false);
            gameManager.LoggedIn();
        }
    }

    /// <summary>
    /// Checking for username availability
    /// </summary>
    /// <param name="username"></param>
    /// <returns>true if is available</returns>
    private async Task<bool> VerifyUsernameAsync(string username)
    {
        string url = "https://maventest-a9cc74b8d5cf.herokuapp.com/webservice/playerinfo/verifyusername?username=" + username;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            UnityWebRequestAsyncOperation operation = webRequest.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string json = webRequest.downloadHandler.text;
                return !bool.Parse(json.ToLower());
            }
            else
            {
                Debug.Log("GET ERROR FOR USERNAME CHECK: " + webRequest.error);
                return false;
            }
        }
    }

    /// <summary>
    /// Checking for email availability
    /// </summary>
    /// <param name="email"></param>
    /// <returns>true if is available</returns>
    private async Task<bool> VerifyEmailAsync(string email)
    {
        string url = "https://maventest-a9cc74b8d5cf.herokuapp.com/webservice/playerinfo/verifyemail?email=" + email;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            UnityWebRequestAsyncOperation operation = webRequest.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string json = webRequest.downloadHandler.text;
                return !bool.Parse(json.ToLower());
            }
            else
            {
                Debug.Log("GET ERROR FOR EMAIL CHECK: " + webRequest.error);
                return false;
            }
        }
    }
}