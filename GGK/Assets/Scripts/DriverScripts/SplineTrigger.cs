using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private bool enterShortcut = false; // True = take shortcut, False = return to main
    private int shortcutNum;

    void Start()
    {
        shortcutNum = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered has the switching component

        SplineBranching incoming = other.GetComponentInParent<SplineBranching>();

        // 50/50 chance that they will get the shortcut
        shortcutNum = Random.Range(0, 100);
        if (shortcutNum <= 0)
        {
            enterShortcut = true;
        }
        else
        {
            enterShortcut = false;
        }


        if (this.CompareTag("Trigger"))
        {
            if (incoming != null)
            {
                if (enterShortcut == true)
                {
                    Debug.Log("switch");
                    incoming.TakeShortcut();
                }
                else
                {
                    Debug.Log("No");
                    incoming.ReturnToMain();

                }
            }
        }
        else if (this.CompareTag("Leave")) //Always leave the shortcut 
        {
            if (incoming != null)
                incoming.ReturnToMain();
        }
        else if (this.CompareTag("SwitchMainSpline"))
        {
            Debug.Log("Switch main");
            incoming.switchMain();
        }
    }

}
