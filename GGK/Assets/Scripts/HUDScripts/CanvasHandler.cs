using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasHandler : MonoBehaviour
{

    #region Variables and Properties

    [SerializeField] protected RawImage itemDisplay;  // The held item (can be empty)

    public static CanvasHandler instance;

    /// <summary>
    /// Read and write
    /// </summary>
    public RawImage ItemDisplay { get { return itemDisplay; } set { itemDisplay = value; } }

    #endregion

    private void Awake()
    {
        instance = this;
    }
}
