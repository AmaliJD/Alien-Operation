using EX;
using MEC;
using PrimeTween;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    public SpriteRenderer sprite_head;
    public SpriteRenderer sprite_back;
    public SpriteRenderer sprite_broken;

    public Patient patient;

    CoroutineHandle colorChangeMEC;
    public int coroutineLayer;
    static int coroutineLayerCounter;

    public Ailment(int timeLimit, float timerSpeed, bool loadNext, float fadeTime, Vector2 location, float offsetDistance = 0, float offsetAngle = 0)
    {
        this.timeLimit = timeLimit;
        this.timerSpeed = timerSpeed;
        this.loadNext = loadNext;
        this.location = location;
        this.textOffset = location + (Vector2.up.Rotate(-offsetAngle) * offsetDistance);

        this.fadeTime = fadeTime;
        coroutineLayerCounter++;
        this.coroutineLayer = coroutineLayerCounter;

        timer = timeLimit;
        //displayTime = Mathf.CeilToInt(timer);
    }

    public void Reset()
    {
        state = AilmentState.Initializing;
        initializing = false;
        atZero = false;
        internalTimer = 0;
        timer = timeLimit;
        SetDisplayTime();
        //Timing.KillCoroutines(coroutineLayer);
        //SetDisplayTextParams(displayTime.ToString(), Fonts.activeFont, Color.red, true);

        //sprite_head.gameObject.SetActive(true);
        //sprite_broken.gameObject.SetActive(false);
        //sprite_broken.transform.localScale = Vector2.one;
    }

    public void SetDisplayTime()
    {
        displayTime = Mathf.CeilToInt(timer);
    }

    public void DecrementTime()
    {
        timer -= Time.deltaTime * timerSpeed;
        int prev_displayTime = displayTime;
        displayTime = Mathf.CeilToInt(timer);
        if (prev_displayTime != displayTime && state == AilmentState.CountDown)
        {
            AudioPlayer.ap.PlaySfx(Ref.clock_tick, .2f, 1.2f);
        }
        displayText.text = timeLimit.ToString();
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
            case AilmentState.Ready:
                SetDisplayTextParams(displayTime.ToString(), Fonts.activeFont, Color.red, newState);
                break;
            case AilmentState.CountDown:
                SetDisplayTextParams(displayTime.ToString(), Fonts.activeFont, Color.red, newState);
                if (newState)
                    AudioPlayer.ap.PlaySfx(Ref.clock_tick, .2f, 1.2f);
                break;
            case AilmentState.Faded:
                SetDisplayTextParams(fadedDisplayTime.ToString(), Fonts.fadedFont, Color.white, newState);
                if (newState)
                    AudioPlayer.ap.PlaySfx(Ref.clock_tick, .2f, 1.2f);
                break;
            case AilmentState.Success:
                SetDisplayTextParams(displayTime.ToString(), Fonts.successFont, new Color(.5f, 1, 0), newState);
                break;
            case AilmentState.Fail_Early:
                SetDisplayTextParams(displayTime.ToString(), Fonts.activeFont, Color.red, newState);
                Break();
                break;
            case AilmentState.Fail_Late:
                SetDisplayTextParams("x", Fonts.activeFont, Color.red, newState);
                Break();
                break;
        }
    }

    void Break()
    {
        sprite_head.gameObject.SetActive(false);
        sprite_broken.gameObject.SetActive(true);
        Tween.Scale(sprite_broken.transform, endValue: Vector2.one * .8f, duration: .25f, ease: Ease.OutBack);
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
        sprite_head = gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();
        sprite_back = gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>();
        sprite_broken = gameObject.transform.GetChild(3).GetComponent<SpriteRenderer>();

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
