using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum AilmentState
{
    Initializing,
    Ready,
    CountDown,
    Faded,
    Success,
    Fail_Early,
    Fail_Late,
}

[Serializable]
public struct PatientSprites
{
    public Sprite body;
    public List<Sprite> faces;
}

public static class Fonts
{
    public static TMP_FontAsset activeFont;
    public static TMP_FontAsset fadedFont;
    public static TMP_FontAsset successFont;
}

public static class Ref
{
    public static GameObject ailmentGameObject;
    public static GameObject patientGameObject;
}
