using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.TestTools;


public class TestBaseItem// : MonoBehaviour 
{

    [Test]
    public void TestCreateBaseItem()
    {
        //baseItem.UseCount { set { UseCount = 1; } }
        BaseItem baseItem = new BaseItem();
        Assert.AreEqual(baseItem.UseCount, baseItem.UseCount);
        //GameObject baseItem = new GameObject();
        //baseItem.AddComponent<BaseItem>(); 
    }

    [Test]
    public void TestSetUseCount()
    {
        BaseItem baseItem = new BaseItem();
        baseItem.UseCount = 1;
        Assert.AreEqual(baseItem.UseCount, 1);
        //Object.DestroyImmediate(baseItem);
    }

    [Test]
    public void TestDecreaseTimer()
    {
        GameObject baseItemTester = new GameObject("BaseItemTester");
        BaseItem baseItem = baseItemTester.AddComponent<BaseItem>();
        baseItem.Timer = 2;
        baseItem.DecreaseTimer();
        Assert.GreaterOrEqual(baseItem.Timer, 1.95f);
        baseItem.Timer = -1f;
        baseItem.DecreaseTimer();
        //Console.WriteLine("Item deleted");
        Assert.AreEqual(baseItem.Timer, -1.0f);
        
    }
}