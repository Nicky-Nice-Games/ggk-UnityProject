using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.TestTools;

public class TestShield
{
    [Test]
    public void TestCreateShield()
    {
        Shield testShield = new Shield();
        Assert.IsInstanceOf<Shield>(testShield);
    }
}