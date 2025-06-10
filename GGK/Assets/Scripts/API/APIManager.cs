using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.Networking;

// If using Python server nav to the server location and use command: python server.py to start

// OUTDATED - Impliment new logis to connect to server. This will NOR work as of current file setup

public class APIManager : MonoBehaviour
{
    private int PID = 0;

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

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Requesting and waiting for the desired data
            yield return webRequest.SendWebRequest();

            // Checking good request
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string data = webRequest.downloadHandler.text;
                PID = int.Parse(data);
            }
            else
            {
                Debug.LogError("Failed to get data" + webRequest.error);
            }
        }
    }

    IEnumerator RunAPIFlow()
    {
        // Sending the request for the PID
        // Currentally getting data from local
        // Swap URI and some logic when remote server is up
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "testData.txt");
        Debug.Log("Path: " + path);

        // Chaining the Coroutines so this one finishes before the main one
        yield return StartCoroutine(GetRequest(path));

        PlayerInfo testPlayer = new PlayerInfo
        {
            playerID = PID,
            raceStartTime = 0330,   // 3:30
            racePosition = 1,
            mapRaced = 2,
            collisionsWithPlayers = 3,
            collisionWithWalls = 4,
            characterUsed = 1,
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

        SerializablePlayerInfo serializable = gameObject.GetComponent<SerializablePlayerInfo>();
        serializable.ConvertToSerializable(testPlayer);
        string json = JsonUtility.ToJson(serializable);

        StartCoroutine(PostJson("http://127.0.0.1:5000/postdata", json));
    }
}
