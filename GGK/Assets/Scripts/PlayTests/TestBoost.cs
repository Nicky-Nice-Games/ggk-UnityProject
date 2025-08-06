using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.TestTools;

public class TestBoost
{
    [Test]
    public void TestCreateBoost()
    {
        Boost testBoost = new Boost();
        Assert.IsInstanceOf<Boost>(testBoost);
    }
}