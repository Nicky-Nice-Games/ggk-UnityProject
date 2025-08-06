using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.TestTools;

public class TestPuck
{
    [Test]
    public void TestCreatePuck()
    {
        Puck testPuck = new Puck();
        Assert.IsInstanceOf<Puck>(testPuck);
    }
}