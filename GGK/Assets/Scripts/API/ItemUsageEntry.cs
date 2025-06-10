using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is a basic container class to act as the serialized dictionary for items
/// </summary>
[System.Serializable]
public class ItemUsageEntry
{
    public string itemName;
    public int usageCount;
}
