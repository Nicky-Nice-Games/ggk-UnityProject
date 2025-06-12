using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class KartCheckpoint : MonoBehaviour
{
    // Start is called before the first frame update
    public int checkpointId;
    public int lap;
    public int placement;
    public string name;
    [SerializeField] List<GameObject> checkpointList;
    [SerializeField]
    private GameObject checkPointParent;
    public float distanceSquaredToNextCP;

    [SerializeField]
    TextMeshProUGUI placementDisplay;
    [SerializeField]
    TextMeshProUGUI lapDisplay;

    void Start()
    {
        checkpointId = 0;
        foreach (Transform child in checkPointParent.GetComponentsInChildren<Transform>(true))
        {
            if (child != checkPointParent.transform) // Avoid adding the parent itself
                checkpointList.Add(child.gameObject);
        }
        if(this.GetComponent<NPCDriver>() == null)
        {
            lapDisplay.text = "Lap: " + (lap + 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        distanceSquaredToNextCP = Mathf.Pow(transform.position.x - checkpointList[(checkpointId + 1) % checkpointList.Count].transform.position.x, 2) +
            Mathf.Pow(transform.position.z - checkpointList[(checkpointId + 1) % checkpointList.Count].transform.position.z, 2);
        if (this.GetComponent<NPCDriver>()== null)
        {
            placementDisplay.text = "Placement: " + placement;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        bool canPass = false;

        if (other.gameObject == checkpointList[(checkpointId + 1) % checkpointList.Count])
        {
            canPass = true;
        }

        //for (int i = 0; i < checkpointId; i++) 
        //{
        //    if (other.gameObject == checkpointList[checkpointId - i])
        //    {
        //        canPass = true;
        //    }
        //}    

        if (other.CompareTag("Checkpoint") && canPass)
        {
            checkpointId++;
        }
        else if (other.CompareTag("Startpoint"))
        {
            if (checkpointId == checkpointList.Count - 1)
            {
                lap++;
                if (this.GetComponent<NPCDriver>() == null)
                {
                    lapDisplay.text = "Lap: " + (lap + 1);
                }
                checkpointId = 0;
            }
        }
    }


}