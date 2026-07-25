using UnityEngine;
using TMPro;
using MEC;
using System.Collections.Generic;
using EX;

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
    public Vector2 textOffset;
    public bool loadNext;

    public AilmentState state;
    public bool complete => state != AilmentState.CountDown && state != AilmentState.Faded && state != AilmentState.Initializing && state != AilmentState.Ready;
    public bool initializing;

    public GameObject gameObject;
    public TextMeshPro displayText;
    public SpriteRenderer sprite;

    public Patient patient;

    CoroutineHandle colorChangeMEC;
    public int couroutineLayer;
    static int couroutineLayerCounter;

    public Ailment(int timeLimit, float timerSpeed, bool loadNext, float fadeTime, Vector2 location, float offsetDistance = 0, float offsetAngle = 0)
    {
        this.timeLimit = timeLimit;
        this.timerSpeed = timerSpeed;
        this.loadNext = loadNext;
        this.location = location;
        this.textOffset = location + (Vector2.up.Rotate(-offsetAngle) * offsetDistance);

        this.fadeTime = fadeTime;
        couroutineLayerCounter++;
        this.couroutineLayer = couroutineLayerCounter;

        timer = timeLimit;
        //displayTime = Mathf.CeilToInt(timer);
    }

    public void SetDisplayTime()
    {
        displayTime = Mathf.CeilToInt(timer);
    }

    public void DecrementTime()
    {
        timer -= Time.deltaTime * timerSpeed;
        displayTime = Mathf.CeilToInt(timer);
        atZero = timer <= 0 && timer > -1;

        internalTimer += Time.deltaTime;
    }

    public void UpdateDisplayText(bool newState = false)
    {
        switch (state)
        {
            case AilmentState.Initializing:
                SetDisplayTextParams("--", null, Color.clear, true);
                break;
            case AilmentState.CountDown:
            case AilmentState.Ready:
                SetDisplayTextParams(displayTime.ToString(), Fonts.activeFont, Color.red, newState);
                break;
            case AilmentState.Faded:
                SetDisplayTextParams(fadedDisplayTime.ToString(), Fonts.fadedFont, Color.white, newState);
                break;
            case AilmentState.Success:
                SetDisplayTextParams(displayTime.ToString(), Fonts.successFont, new Color(.5f, 1, 0), newState);
                break;
            case AilmentState.Fail_Early:
                SetDisplayTextParams(displayTime.ToString(), Fonts.activeFont, Color.red, newState);
                break;
            case AilmentState.Fail_Late:
                SetDisplayTextParams("x", Fonts.activeFont, Color.red, newState);
                break;
        }
    }

    void SetDisplayTextParams(string text, TMP_FontAsset font, Color color, bool newState)
    {
        displayText.text = text;
        displayText.font = font;

        if (newState)
            displayText.color = color;
    }

    public void InitGameObject()
    {
        gameObject = GameObject.Instantiate(Ref.ailmentGameObject, patient.gameObject.transform);
        displayText = gameObject.transform.GetChild(0).GetComponent<TextMeshPro>();
        sprite = gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();

        gameObject.transform.localPosition = location;
        displayText.transform.localPosition = textOffset - location;
        displayText.text = timeLimit.ToString();
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

    public IEnumerator<float> _FadeOut(float fadeDuration)
    {
        float alpha = 1;
        float time = fadeDuration;
        while (time > 0)
        {
            alpha = time / fadeDuration;
            displayText.color = new Color(displayText.color.r, displayText.color.g, displayText.color.b, alpha);

            yield return Timing.WaitForOneFrame;
            time -= Time.deltaTime;
        }
    }

    public IEnumerator<float> _HoldFadeOut(float holdDuration, float fadeDuration)
    {
        float alpha = 1;
        float time = holdDuration + fadeDuration;
        while (time > 0)
        {
            alpha = Mathf.Clamp01(time / fadeDuration);
            displayText.color = new Color(displayText.color.r, displayText.color.g, displayText.color.b, alpha);

            yield return Timing.WaitForOneFrame;
            time -= Time.deltaTime;
        }
    }

    public IEnumerator<float> _FlashHoldFadeOut(float flashSpeed, Color color1, Color color2, float holdDuration, float fadeDuration)
    {
        float alpha = 1;
        float time = holdDuration + fadeDuration;
        float flashTime = 0;
        bool colortoggle = false;
        while (time > 0)
        {
            if (flashTime >= flashSpeed)
            {
                colortoggle = !colortoggle;
                flashTime = 0;
            }

            alpha = Mathf.Clamp01(time / fadeDuration);
            if (!colortoggle)
            {
                displayText.color = new Color(color1.r, color1.g, color1.b, alpha);
            }
            else
            {
                displayText.color = new Color(color2.r, color2.g, color2.b, alpha);
            }

            yield return Timing.WaitForOneFrame;
            flashTime += Time.deltaTime;
            time -= Time.deltaTime;
        }
    }
}
