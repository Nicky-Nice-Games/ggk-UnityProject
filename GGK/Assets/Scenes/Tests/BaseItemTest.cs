using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseItemTest
{

    [UnityTest]
    //public GameObject GettingMockBaseItem()
    //{
        //return Substitute.For<BaseItem>();
    //}

    //private GameObject baseItemTester;
    //private BaseItem baseItem;
    [SetUp]
    public void Setup()
    {
        // This method is called before each test is run.
        // You can use it to set up any necessary objects or state.
        //baseItemTester = new GameObject("BaseItemTester");
        //BaseItem baseItem = baseItemTester.AddComponent<BaseItem>();

    //}
    //[TearDown]
    //public void Teardown()
    //{
        // This method is called after each test is run.
        // You can use it to clean up any objects or state.
        //Object.Destroy(baseItemTester);
    //}

    // A Test behaves as an ordinary method
    //[UnityTest]
    //public IEnumerator BaseItem_DecreaseTimer_Test()
    //{
    //    BaseItem baseItem = baseItemTester.AddComponent<BaseItem>();
    //    float startTime = 2.0f; // Set the initial timer to 2 seconds
    //    baseItem.timer = startTime;

    //    yield return null;

    //    baseItem.DecreaseTimer();
    //   float  expectedTime = startTime - 1.0f * Time.deltaTime; // Expected time after decreasing by 1 second
    //                                                            // Assert that the timer has decreased by approximately 1 second
    //    Assert.Less(baseItem.timer, startTime, "Timer should decrease");
    //    Assert.AreEqual(expectedTime, baseItem.timer, 0.001f, "Timer should decrease by deltaTime");

    //}

    //[Test]
    //public void BaseItem_IsUpgraded_Test()
    //{
        // Test the default value of IsUpgraded
        //Assert.IsFalse(baseItem.IsUpgraded, "Default value of IsUpgraded should be false");
        // Set IsUpgraded to true and test
        //baseItem.IsUpgraded = true;
        //Assert.IsTrue(baseItem.IsUpgraded, "IsUpgraded should be true after setting it to true");
        // Set IsUpgraded back to false and test
        //baseItem.IsUpgraded = false;
        //Assert.IsFalse(baseItem.IsUpgraded, "IsUpgraded should be false after setting it to false");
    }




}
