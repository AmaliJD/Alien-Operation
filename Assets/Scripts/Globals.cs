using UnityEngine;
using TMPro;

public enum AilmentState
{
    CountDown,
    Faded,
    Success,
    Fail_Early,
    Fail_Late,
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
}
