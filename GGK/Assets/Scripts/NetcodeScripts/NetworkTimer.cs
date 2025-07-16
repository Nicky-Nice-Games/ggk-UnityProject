public class NetworkTimer
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
