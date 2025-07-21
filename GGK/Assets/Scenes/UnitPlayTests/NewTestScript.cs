using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void NewTestScriptSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        //BaseItem baseItem;
        yield return null;
    }

    [UnityTest]
    public IEnumerator MonoBehaviourTest_Works()
    {
        yield return new MonoBehaviourTest<MyMonoBehaviourTest>();
    }

    public class MyMonoBehaviourTest : MonoBehaviour, IMonoBehaviourTest
    {
        private int frameCount;
        private GameObject gameObject = new GameObject();
        public bool IsTestFinished
        {
            get { return frameCount > 10; }
        }

        void Update()
        {
            frameCount++;
            gameObject.AddComponent<BaseItem>();
        }
    }
}
