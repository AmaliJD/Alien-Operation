using UnityEngine;
using TMPro;

public class Ailment
{
    public int timeLimit;
    public float timer;
    public float timerSpeed;
    public int displayTime;
    public int fadedDisplayTime;
    float internalTimer;
    float fadeTime;
    public bool atZero;
    public Vector2 location;
    public bool loadNext;

    public AilmentState state;
    public bool complete => state != AilmentState.CountDown && state != AilmentState.Faded;

    public GameObject ailmentGameObject;
    public TextMeshPro ailmentDisplayText;

    public Ailment(int timeLimit, float timerSpeed, bool loadNext, float fadeTime, float x = 0, float y = 0)
    {
        this.timeLimit = timeLimit;
        this.timerSpeed = timerSpeed;
        this.loadNext = loadNext;
        this.location = new Vector2(x, y);

        this.fadeTime = fadeTime;

        timer = timeLimit;
        displayTime = Mathf.CeilToInt(timer);
    }

    public void DecrementTime()
    {
        timer -= Time.deltaTime * timerSpeed;
        displayTime = Mathf.CeilToInt(timer);
        atZero = timer <= 0 && timer > -1;

        internalTimer += Time.deltaTime;
    }

    public void UpdateDisplayText()
    {
        ailmentDisplayText.text = state switch
        {
            AilmentState.CountDown => displayTime.ToString(),
            AilmentState.Faded => fadedDisplayTime.ToString(),
            AilmentState.Success => displayTime.ToString(),
            AilmentState.Fail_Early => displayTime.ToString(),
            AilmentState.Fail_Late => "x",
        };

        ailmentDisplayText.font = state switch
        {
            AilmentState.CountDown => Fonts.activeFont,
            AilmentState.Faded => Fonts.fadedFont,
            AilmentState.Success => Fonts.successFont,
            AilmentState.Fail_Early => Fonts.activeFont,
            AilmentState.Fail_Late => Fonts.activeFont,
        };
    }

    public void InitGameObject()
    {
        ailmentGameObject = GameObject.Instantiate(Ref.ailmentGameObject);
        ailmentDisplayText = ailmentGameObject.transform.GetChild(0).GetComponent<TextMeshPro>();

        ailmentGameObject.transform.position = location;
        ailmentDisplayText.text = timeLimit.ToString();
    }

    public bool AtFadeTime()
    {
        if (fadeTime == 0)
            return false;
        else if (fadeTime > 0)
            return internalTimer >= fadeTime;
        else
            return timer <= Mathf.Abs(fadeTime);
    }
}
