using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

// Script relies on the data.json being in the same directory as API

public class APIManager : MonoBehaviour
{

    void Start()
    {
        //StartCoroutine(RunAPIFlow());
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

        // Checks the result of teh request 
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data posted successfully!");
        }
        else
        {
            Debug.LogError("Error posting data: " + request.error);
        }
    }

    private IEnumerator GetRequest(string uri, PlayerInfo thisPlayer)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
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

    /// <summary>
    /// Wrapper method for running the API manager flow
    /// </summary>
    /// <returns></returns>
    /*
    IEnumerator RunAPIFlow()
    {
        // Get info from user login
        // This will have to set the pid so we can communicate with backend
        string placeHolderPID = "26ec3c18-3fe5-11f0-8cc9-ac1f6bbcd350";

        // Sending the request for the existing player info (gameservice/gamelog/player/{pid})
        string getPath = "https://maventest-a9cc74b8d5cf.herokuapp.com/gameservice/gamelog/player/" + placeHolderPID;    
        //Debug.Log("Path: " + getPath);

        // Chaining the Coroutines so this one finishes before the main one
        // Sending request for existing player data
        yield return StartCoroutine(GetRequest(getPath));

        // testing player ---
        
        PlayerInfo testPlayer = new PlayerInfo
        {
            playerID = 0,
            raceStartTime = 0330,   // 3:30
            racePosition = 1,
            mapRaced = 2,
            collisionsWithPlayers = 3,
            collisionWithWalls = 4,
            characterUsed = 1,
            fellOffMap = 2,
            boostUsage =
            {
                ["Speed Boost 1"] = 5,
                ["Speed Boost 2"] = 6,
                ["Speed Boost 3"] = 2
            },
            offenceUsage =
            {
                ["Puck 1"] = 4,
                ["Puck 2"] = 2,
            },
            trapUsage =
            {
                ["Oil spill 1"] = 3,
                ["Oil spill 2"] = 1
            }
        };
        
        // ---

        // UPDATE PLAYER STATS FOR THE MATCH (after race ended)

        // Serializing data to send back
        SerializablePlayerInfo serializable = gameObject.GetComponent<SerializablePlayerInfo>();
        serializable.ConvertToSerializable(new PlayerInfo());   // Placeholder player info value CHANGE WHEN ABLE TO
        string json = JsonUtility.ToJson(serializable);

        // Post the updated data struct
        // Updated to send post
        //StartCoroutine(PostJson("https://maventest-a9cc74b8d5cf.herokuapp.com/gameservice/gamelog", json));     // UPDATE URI IF NEEDED
    }
    */

    // Starts the corutine for posting json data
    public void SendPost(string json)
    {
        StartCoroutine(PostJson("https://maventest-a9cc74b8d5cf.herokuapp.com/gameservice/gamelog", json));    
    }

    /// <summary>
    /// Starts the corutine for getting json data
    /// </summary>
    /// <param name="pid">the player ID</param>
    /// <param name="thisPlayer">the player whos data will be set</param>
    /// <returns></returns>
    public IEnumerator SendGet(string pid, PlayerInfo thisPlayer)
    {
        // Sending the request for the existing player info (gameservice/gamelog/player/{pid})
        string getPath = "https://maventest-a9cc74b8d5cf.herokuapp.com/gameservice/gamelog/player/" + pid;
        Debug.Log("Path: " + getPath);
        yield return StartCoroutine(GetRequest(getPath, thisPlayer));
    }

    /// <summary>
    /// Using email to check if player exists in online database
    /// </summary>
    /// <param name="email">email</param>
    /// <param name="callback">the bool that will be returned as a callback</param>
    /// <returns></returns>
    public IEnumerator CheckForPlayerInData(string email, Action<bool> callback)
    {
        // uri = gameservice/playerlog/{email}
        using (UnityWebRequest webRequest = UnityWebRequest.Get(
            "https://maventest-a9cc74b8d5cf.herokuapp.com/gameservice/playerlog/" + email))
        {
            yield return webRequest.SendWebRequest();

            // Checking good request
            if(webRequest.result == UnityWebRequest.Result.Success)
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
