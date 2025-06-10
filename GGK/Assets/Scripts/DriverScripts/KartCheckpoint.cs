using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KartCheckpoint : MonoBehaviour
{
    // Start is called before the first frame update
    public int checkpointId;
    public int lap;
    public int placement;
    public string name;
    [SerializeField] List<GameObject> checkpointList = new List<GameObject>();
    public float distanceSquaredToNextCP;
    void Start()
    {
        checkpointId = 0;
    }

    // Update is called once per frame
    void Update()
    {
        distanceSquaredToNextCP = Mathf.Pow(transform.position.x - checkpointList[(checkpointId + 1) % 9].transform.position.x, 2) +
            Mathf.Pow(transform.position.z - checkpointList[(checkpointId + 1) % 9].transform.position.z, 2);
    }

    private void OnTriggerEnter(Collider other)
    {

        bool canPass = false;

        if (other.gameObject == checkpointList[(checkpointId + 1) % 9])
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
                checkpointId = 0;
            }
        }
    }


}
