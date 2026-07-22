using UnityEngine;

public class Ailment
{
    public int timeLimit;
    public float timer;
    public float timerSpeed;
    public int displayTime;
    public bool atZero;
    public Vector2 location;
    public bool loadNext;

    public AilmentState state;
    public bool complete => state != AilmentState.CountDown;

    public Ailment(int timeLimit, float timerSpeed, bool loadNext, float x = 0, float y = 0)
    {
        this.timeLimit = timeLimit;
        this.timerSpeed = timerSpeed;
        this.loadNext = loadNext;
        this.location = new Vector2(x, y);

        timer = timeLimit;
        displayTime = Mathf.CeilToInt(timer);
    }

    public void DecrementTime()
    {
        timer -= Time.deltaTime * timerSpeed;
        displayTime = Mathf.CeilToInt(timer);
        atZero = timer <= 0 && timer > -1;
    }
}
