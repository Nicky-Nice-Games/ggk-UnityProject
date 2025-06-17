using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.Networking;

// Script relies on the data.json being in the same directory as API

public class APIManager : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(RunAPIFlow());
    }

    /// <summary>
    /// Setting up the post request
    /// </summary>
    /// <param name="url">where to send the data</param>
    /// <param name="jsonData">json data</param>
    /// <returns></returns>
    IEnumerator PostJson(string url, string jsonData)
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

    // Get endpoint on local maven server should be sm like http://localhost:8080/getdata
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Requesting and waiting for the desired data
            yield return webRequest.SendWebRequest();

            // Checking good request
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // UPDATE THIS SO GOOD REQUEST RETURNS PLAYERINFO DATA STRUCT WITH PLAYER INFO COPIED OVER
                // Getting the data from the json get request
                string data = webRequest.downloadHandler.text;
                WebUserData userData = JsonUtility.FromJson<WebUserData>(data);
                Debug.Log("PID: " + userData.pid);
            }
            else
            {
                // UPDATE THIS SO BAD REQUEST / NULL OBJ RETURNED (CHECK WITH BACKEND ON THAT) RETURNS CONSTRUCTED DEFAULT PLAYER INFO DATA STRUCT
                Debug.LogError("Failed to get data" + webRequest.error);
            }
        }
    }

    /// <summary>
    /// Wrapper method for running the API manager flow
    /// </summary>
    /// <returns></returns>
    IEnumerator RunAPIFlow()
    {
        // Get info from user login

        // Sending the request for the existing player info (gameservice/gamelog/player/{pid})
        string getPath = "https://maventest-a9cc74b8d5cf.herokuapp.com/webservice/playerinfo/getinfo/26ec3c18-3fe5-11f0-8cc9-ac1f6bbcd350";    // UPDATE THIS
        Debug.Log("Path: " + getPath);

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

        // UPDATE PLAYER STATS FOR THE MATCH

        // Serializing data to send back
        SerializablePlayerInfo serializable = gameObject.GetComponent<SerializablePlayerInfo>();
        serializable.ConvertToSerializable(testPlayer);
        string json = JsonUtility.ToJson(serializable);

        // Post the updated data struct
        StartCoroutine(PostJson("https://maventest-a9cc74b8d5cf.herokuapp.com/gameservice/gamelog", json));     // UPDATE URI IF NEEDED
    }
}
