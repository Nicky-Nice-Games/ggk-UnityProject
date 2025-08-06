using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.TestTools;

public class TestTrapItem
{
    [Test]
    public void TestCreateTrapItem()
    {
        TrapItem testTrapItem = new TrapItem();
        Assert.IsInstanceOf<TrapItem>(testTrapItem);
    }
}