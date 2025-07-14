using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{

    void Start()
    {

    }

    /// <summary>
    /// Setting up the post request
    /// </summary>
    /// <param name="url">where to send the data</param>
    /// <param name="jsonData">json data</param>
    /// <returns></returns>
    private IEnumerator PostJson(string url, string jsonData)
    {
        // Sets up the unity request
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Sends back each data at a time
        yield return request.SendWebRequest();

        // Checks the result of the request 
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data posted successfully!");
        }
        else
        {
            Debug.LogError("Error posting data: " + request.error);
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

        StartCoroutine(PostJson("https://maventest-a9cc74b8d5cf.herokuapp.com/gameservice/gamelog", json));
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
    /// Using email to check if player exists in online database
    /// </summary>
    /// <param name="email">email</param>
    /// <param name="callback">the bool that will be returned as a callback</param>
    /// <returns></returns>
    public IEnumerator CheckForPlayerInData(string email, Action<bool> callback)
    {
        // uri = gameservice/playerlog/{email} when checking with email
        using (UnityWebRequest webRequest = UnityWebRequest.Get(
            "https://maventest-a9cc74b8d5cf.herokuapp.com/gameservice/playerlog/" + email))
        {
            yield return webRequest.SendWebRequest();

            // Checking good request
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string json = webRequest.downloadHandler.text;

                // Getting the bool out of the returned data
                callback(bool.Parse(json.ToLower()));
            }
            else
            {
                Debug.Log("GET ERROR FOR PLAYER CHECK: " + webRequest.error);
                callback(false);
            }
        }
    }
}