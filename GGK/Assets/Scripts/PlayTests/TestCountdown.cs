using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.TestTools;

public class TestCountdown
{
    [Test]
    public void TestCreateCountdown()
    {
        Countdown testCountdown = new Countdown();
        Assert.IsInstanceOf<Countdown>(testCountdown);
    }

    [Test]
    public void TestCountDown()
    {
        GameObject countdownTester = new GameObject("Countdown Tester");
        Countdown testCountdown = countdownTester.AddComponent<Countdown>();
        testCountdown.countdownCount = 5;
        //testCountdown.Start();
        //Assert.AreEqual(testCountdown.countdownCount, 1);
    }
}