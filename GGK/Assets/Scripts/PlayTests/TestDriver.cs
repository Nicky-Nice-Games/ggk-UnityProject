using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.TestTools;

public class TestDriver
{
    [Test]
    public void TestCreateDriver()
    {
        Driver testDriver = new Driver();
        Assert.IsInstanceOf<Driver>(testDriver);
    }
}