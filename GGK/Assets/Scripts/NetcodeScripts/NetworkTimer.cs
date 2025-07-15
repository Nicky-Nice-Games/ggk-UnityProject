using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class NetworkTimer : MonoBehaviour
{
    private float timer;

    public float MinimumTimeBetweenTicks { get; }
    public int CurrentTick { get; private set; }

    public NetworkTimer(float serverTickRate) {
        MinimumTimeBetweenTicks = 1f / serverTickRate;
    }

    public void Update(float deltaTime){
        timer += deltaTime;
    }

    public bool ShouldTick() {
        if (timer >= MinimumTimeBetweenTicks){
            timer -= MinimumTimeBetweenTicks;
            CurrentTick++;
            return true;
        }
        return false;
    }
}
