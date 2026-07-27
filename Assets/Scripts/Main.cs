using GLDebug;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using MEC;
using PrimeTween;

public class Main : MonoBehaviour
{
    List<Patient> patients = new();
    int patientIndex = -1;

    const float AIL_RADIUS = 1.15f;
    bool readMouseDown;

    [Header("Globals")]
    public TMP_FontAsset ref_activeFont;
    public TMP_FontAsset ref_fadedFont;
    public TMP_FontAsset ref_successFont;
    public GameObject ref_ailmentGameObject;
    public GameObject ref_patientGameObject;

    [Header("Patient Sprites")]
    public List<PatientSprites> patientSprites;

    [Header("Audio")]
    [SerializeField] AudioClip failSfx;
    [SerializeField] AudioClip successSfx;
    [SerializeField] AudioClip[] hurtSfx;
    [SerializeField] AudioClip you_win;
    [SerializeField] AudioClip you_win_perfect;
    [SerializeField] AudioClip ref_clock_tick;

    [Header("UI")]
    public Transform ui_title;
    public TextMeshProUGUI ui_title_text;
    public Transform ui_you_win;
    public Transform ui_perfect;
    public RectTransform ui_icons;
    public RectTransform ui_x;
    public TrailRenderer trail;

    public int lives;
    bool start;
    bool perfectRun = true;
    bool perfectPatient = true;
    bool win;
    bool win_click;

    int frame_count = -1;

    void Awake()
    {
        Fonts.activeFont = ref_activeFont;
        Fonts.fadedFont = ref_fadedFont;
        Fonts.successFont = ref_successFont;
        Ref.ailmentGameObject = ref_ailmentGameObject;
        Ref.patientGameObject = ref_patientGameObject;
        Ref.clock_tick = ref_clock_tick;

        Vector2 mousePos_1 = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        trail.transform.position = mousePos_1;

        Init();
    }

    void Init()
    {
        patients.Clear();

        foreach (Transform t in ui_x)
        {
            t.GetComponent<TextMeshProUGUI>().text = "";
        }

        NewPatient().AddAilments(
            new Ailment(5, 1f, false, 0, new Vector2(0, 0), AIL_RADIUS * 1.3f, 45)
        );

        NewPatient().AddAilments(
            new Ailment(5, 1.667f, true, 0, new Vector2(0f, 1.7f), AIL_RADIUS * 1.3f, 90),
            new Ailment(3, 1f, false, 0, new Vector2(1.4f, -1.5f), AIL_RADIUS * 1.3f, -90)
        );

        NewPatient().AddAilments(
            new Ailment(5, 1f, false, -2, new Vector2(-2.4f, 2.4f), AIL_RADIUS * 1.3f, -90),
            new Ailment(5, 1.5f, false, -2, new Vector2(1.2f, 1.3f), AIL_RADIUS * 1.3f, 45),
            new Ailment(10, 2f, false, -4, new Vector2(-.3f, 0), AIL_RADIUS * 1.9f, -135)
        );

        NewPatient().AddAilments(
            new Ailment(5, 1.2f, false, -3, new Vector2(-.5f, 3f), AIL_RADIUS * 1.3f, 90),
            new Ailment(5, 1.667f, true, -1, new Vector2(-2.25f, 1f), AIL_RADIUS * 1.3f, 90),
            new Ailment(3, 1f, false, -1, new Vector2(.6f, -.55f), AIL_RADIUS * 1.3f, 135)
        );

        NewPatient().AddAilments(
            new Ailment(5, 1.2f, true, -2, new Vector2(-.4f, -2.4f), AIL_RADIUS * 1.3f, -90),
            new Ailment(5, 1.2f, false, -3, new Vector2(1.3f, -1.4f), AIL_RADIUS * 1.3f, 90),
            new Ailment(3, 1f, true, -1, new Vector2(0f, .6f), AIL_RADIUS * 1.3f, 90),
            new Ailment(3, 2f, false, -1, new Vector2(-2f, 1.7f), AIL_RADIUS * 1.3f, -90),
            new Ailment(10, 2f, false, -6, new Vector2(2f, 1.7f), AIL_RADIUS * 2f, 90)
        );

        NewPatient().AddAilments(
            new Ailment(10, 3f, false, -2, new Vector2(0, 3), AIL_RADIUS * 1.5f, 180),
            new Ailment(6, 3f, false, -2, new Vector2(0, 1.5f), AIL_RADIUS * 1.5f, 180),
            new Ailment(4, 3f, false, -2, new Vector2(0, 0), AIL_RADIUS * 1.5f, 0),
            new Ailment(2, 2f, true, -1, new Vector2(-2, 0), AIL_RADIUS * 1.3f, -90),
            new Ailment(2, 1f, true, -1, new Vector2(2, 0), AIL_RADIUS * 1.3f, 90)
        );

        NewPatient().AddAilments(
            new Ailment(5, 1f, true, 2, new Vector2(-.5f, 0), AIL_RADIUS * 1.5f, 180),
            new Ailment(10, 2f, true, 2, new Vector2(-.4f, 2.5f), AIL_RADIUS * 2f, -90),
            new Ailment(15, 3f, false, 2, new Vector2(1.4f, 1.75f), AIL_RADIUS * 2f, 90),
            new Ailment(15, 2f, true, -10, new Vector2(.1f, 1.3f), AIL_RADIUS * 1.7f, -135)
        );

        NewPatient().AddAilments(
            new Ailment(10, 1f, true, -7, new Vector2(-.8f, 1.5f/*-1f, -1.6f*/), AIL_RADIUS * 2f, -60),
            new Ailment(5, 1f, true, -2, new Vector2(-1f, 0f), AIL_RADIUS * 1.5f, -90),
            new Ailment(12, 1.2f, false, -4, new Vector2(1.5f, 1.8f), AIL_RADIUS * 2f, 45), //
            new Ailment(12, 1.2f, true, -2, new Vector2(-2.7f, 2f), AIL_RADIUS * 2f, -135),
            new Ailment(12, 1.5f, false, -2, new Vector2(3f, 3f), AIL_RADIUS * 2f, 135), //
            new Ailment(10, 1.5f, false, -8, new Vector2(2.1f, .4f), AIL_RADIUS * 2f, 135), //
            new Ailment(20, 4f, false, -5, new Vector2(-1.5f, -1.75f), AIL_RADIUS * 2f, -30), //
            new Ailment(20, 8f, false, -5, new Vector2(-3f, -3f), AIL_RADIUS * 2f, -90) //
        );

        lives = 3;
        patientIndex = -1;

        start = true;
        perfectRun = true;
        perfectPatient = true;

        win = false;
        win_click = false;
    }

    void SetX(int i)
    {
        TextMeshProUGUI ui_x_i = ui_x.GetChild(i).GetComponent<TextMeshProUGUI>();
        ui_x_i.text = "x";
        ui_x_i.color = new Color(1, 0, 0, ui_x_i.color.a);
    }

    void SetP(int i)
    {
        TextMeshProUGUI ui_x_i = ui_x.GetChild(i).GetComponent<TextMeshProUGUI>();
        ui_x_i.text = "P";
        ui_x_i.color = new Color(1, .9f, 0, ui_x_i.color.a);
    }

    Patient NewPatient()
    {
        Patient p = new();
        patients.Add(p);

        return p;
    }

    void Update()
    {
        frame_count++;

        Vector2 mousePos_1 = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        trail.transform.position = mousePos_1;

        if (frame_count == 2)
        {
            trail.emitting = true;
            trail.time = .1f;
        }

        if (win)
            return;

        if (Keyboard.current.leftShiftKey.value == 1 && Keyboard.current.nKey.wasPressedThisFrame)
        {
            if (patientIndex < patients.Count - 1)
            {
                if (patientIndex > 0)
                    perfectRun = false;

                LoadNextPatient();
            }
            else if (patientIndex == patients.Count - 1)
            {
                if (patientIndex > 0)
                    perfectRun = false;

                LoadWin();
            }
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame && patientIndex >= 0)
        {
            Esc();
        }

        if (patientIndex < 0)
            return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        bool mousePressed = Mouse.current.leftButton.value == 1;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            readMouseDown = true;

        Patient patient = patients[patientIndex];
        if (!patient.finished)
        {
            if (patient.IsCurrentFinished())
            {
                readMouseDown = false;
                patient.LoadNextAilments();
            }

            foreach (Ailment ail in patient.currentAilments)
            {
                Vector2 ail_location = ail.gameObject.transform.position;
                bool cursorOnAil = Vector2.Distance(mousePos, ail_location) <= AIL_RADIUS;

                if (ail.state == AilmentState.Initializing)
                {
                    if (!ail.initializing)
                    {
                        ail.initializing = true;
                        Timing.KillCoroutines(ail.coroutineLayer);
                        ail.displayText.color = Color.red;
                        //Tween.Scale(ail.displayText.transform, endValue: Vector2.one, duration: .25f, ease: Ease.OutBack, startDelay: patientIndex > 0 ? 1f : 1.5f);
                        //Tween.Scale(ail.sprite_head.transform, endValue: Vector2.one, duration: .25f, ease: Ease.OutBack, startDelay: patientIndex > 0 ? 1f : 1.5f)
                        //    .OnComplete(() =>
                        //    {
                        //        ail.state = AilmentState.Ready;
                        //        ail.SetDisplayTime();
                        //    });
                        Sequence.Create()
                            .Group(Tween.Scale(ail.displayText.transform, endValue: Vector2.one, duration: .25f, ease: Ease.OutBack, startDelay: patientIndex > 0 ? 1f : 1.5f))
                            .Group(Tween.Scale(ail.sprite_head.transform, endValue: Vector2.one, duration: .25f, ease: Ease.OutBack, startDelay: patientIndex > 0 ? 1f : 1.5f))
                            //.Group(Tween.Delay(duration: .25f, () =>
                            //{
                            //    ail.state = AilmentState.Ready;
                            //    ail.SetDisplayTime();
                            //}));
                            //.ChainDelay(.25f)
                            .OnComplete(() =>
                            {
                                ail.state = AilmentState.Ready;
                                ail.SetDisplayTime();
                            });
                        //Sequence.Create()
                        //    .ChainDelay(.25f)
                        //    .OnComplete(() =>
                        //    {
                        //        ail.state = AilmentState.Ready;
                        //        ail.SetDisplayTime();
                        //    });
                        continue;
                    }
                    continue;
                }

                if (ail.complete)
                    continue;

                AilmentState currentAilstate = ail.state;
                if (ail.state == AilmentState.Ready)
                    ail.state = AilmentState.CountDown;

                ail.DecrementTime();

                Color gizmoColor = ail.state switch
                {
                    AilmentState.CountDown => (cursorOnAil ? new Color(1, .5f, 0) : Color.red),
                    AilmentState.Faded => (cursorOnAil ? new Color(1, .5f, 0) : Color.red),
                    AilmentState.Success => new Color(.2f, 1, 0),
                    _ => Color.red
                };
                gizmoColor.a = .3f;

                GLGizmos.SetColor(gizmoColor);
                GLGizmos.DrawWeightedCircle(ail_location, AIL_RADIUS, .1f, BorderType.Inside, -2);

                GLGizmos.DrawSolidCircle(ail_location, AIL_RADIUS - .2f, -2)
                    .SetColor(new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, cursorOnAil ? .1f : .05f));

                if (ail.displayTime == 0 && ail.state == AilmentState.CountDown && ail.displayText.color != Color.white)
                {
                    ail.displayText.color = Color.white;
                    Tween.ShakeLocalPosition(ail.displayText.transform, strength: Vector2.one * .075f, frequency: 20, duration: ail.timerSpeed);
                }

                if (ail.AtFadeTime() && ail.state == AilmentState.CountDown)
                {
                    ail.state = AilmentState.Faded;
                    ail.fadedDisplayTime = ail.displayTime;
                    Timing.KillCoroutines(ail.coroutineLayer);
                    Timing.RunCoroutine(ail._HoldFadeOut(.1f, 2f), ail.coroutineLayer);
                }
                if (ail.displayTime < 0)
                {
                    ail.state = AilmentState.Fail_Late;
                    Timing.KillCoroutines(ail.coroutineLayer);
                    Timing.RunCoroutine(ail._FlashHoldFadeOut(.1f, Color.white, Color.red, .5f, 1f), ail.coroutineLayer);
                    AudioPlayer.ap.PlaySfx(failSfx, 1);
                    PlayRandomHurtSfx();

                    Timing.KillCoroutines(patient.gameObject);
                    Timing.RunCoroutine(patient._SwapFaces(1, patient.lives > 1 ? 0 : 2), patient.gameObject);

                    patient.body_hit.color = Color.red;
                    patient.face_hit.color = Color.red;
                    Sequence.Create()
                        .ChainDelay(patient.lives > 1 ? 0f : .5f)
                        .Group(Tween.Alpha(patient.body_hit, endValue: 0, duration: patient.lives > 1 ? .6f : 1f))
                        .Group(Tween.Alpha(patient.face_hit, endValue: 0, duration: patient.lives > 1 ? .6f : 1f))
                        .Group(Tween.ShakeLocalPosition(patient.gameObject.transform, strength: Vector2.one * (patient.lives > 1 ? .1f : .25f), duration: 1f));

                    patient.ailmentsHit++;
                    patient.lives--;

                    if (patientIndex > 0)
                    {
                        perfectRun = false;
                        perfectPatient = false;
                    } 
                }
                else if (mousePressed && cursorOnAil && readMouseDown)
                {
                    if (ail.displayTime == 0)
                    {
                        ail.state = AilmentState.Success;
                        Timing.KillCoroutines(ail.coroutineLayer);
                        Timing.RunCoroutine(ail._FlashHoldFadeOut(.1f, new Color(.5f, 1, 0), new Color(.9f, 1, .75f), 1f, .25f), ail.coroutineLayer);
                        AudioPlayer.ap.PlaySfx(successSfx, .8f);

                        Timing.KillCoroutines(patient.gameObject);

                        patient.body_hit.color = new Color(.5f, 1, 0);
                        patient.face_hit.color = new Color(.5f, 1, 0);
                        Sequence.Create()
                            .Group(Tween.Alpha(patient.body_hit, endValue: 0, duration: .6f))
                            .Group(Tween.Alpha(patient.face_hit, endValue: 0, duration: .6f));

                        Tween.Scale(ail.sprite_head.transform, endValue: Vector2.zero, duration: .25f, ease: Ease.InBack);
                        Tween.Scale(ail.sprite_back.transform, endValue: Vector2.zero, duration: .25f, ease: Ease.InBack);

                        patient.ailmentsHit++;
                        if (patient.finished)
                            patient.lastSuccess = true;
                    }
                    else
                    {
                        ail.state = AilmentState.Fail_Early;
                        Timing.KillCoroutines(ail.coroutineLayer);
                        Timing.RunCoroutine(ail._FlashHoldFadeOut(.1f, Color.white, Color.red, .5f, 1f), ail.coroutineLayer);
                        AudioPlayer.ap.PlaySfx(failSfx, 1);
                        PlayRandomHurtSfx();

                        Timing.KillCoroutines(patient.gameObject);
                        Timing.RunCoroutine(patient._SwapFaces(1, patient.lives > 1 ? 0 : 2), patient.gameObject);

                        patient.body_hit.color = Color.red;
                        patient.face_hit.color = Color.red;
                        Sequence.Create()
                            .ChainDelay(patient.lives > 1 ? 0f : .5f)
                            .Group(Tween.Alpha(patient.body_hit, endValue: 0, duration: patient.lives > 1 ? .6f : 1f))
                            .Group(Tween.Alpha(patient.face_hit, endValue: 0, duration: patient.lives > 1 ? .6f : 1f))
                            .Group(Tween.ShakeLocalPosition(patient.gameObject.transform, strength: Vector2.one * (patient.lives > 1 ? .1f : .25f), duration: 1f));

                        patient.ailmentsHit++;
                        patient.lives--;

                        if (patientIndex > 0)
                        {
                            perfectRun = false;
                            perfectPatient = false;
                        }
                    }
                }

                ail.UpdateDisplayText(currentAilstate != ail.state);
            }
        }
        else
        {
            if(patient.next == 0)
            {
                if (patient.lives > 0 && patient.lastSuccess)
                {
                    Timing.KillCoroutines(patient.gameObject);
                    patient.SetFace(2);

                    if (perfectPatient && patientIndex > 0)
                    {
                        SetP(patientIndex - 1);
                    }
                }
                else if (patient.lives <= 0)
                {
                    Timing.KillCoroutines(patient.gameObject);
                    patient.SetFace(3);

                    if (patientIndex > 0)
                    {
                        lives--;
                        SetX(patientIndex - 1);
                    } 
                }

                Timing.RunCoroutine(patient._Next(), patient.gameObject);
            }
            else if (patient.next == 2)
            {
                if (patientIndex < patients.Count - 1)
                {
                    LoadNextPatient();
                }
                else if (patientIndex == patients.Count - 1)
                {
                    if (lives > 0)
                        LoadWin();
                    else
                        LoadNextPatient();
                }
            }
        }
    }

    void PlayRandomHurtSfx()
    {
        int i = UnityEngine.Random.Range(0, hurtSfx.Length);
        AudioPlayer.ap.PlaySfx(hurtSfx[i], .6f);
    }

    void Esc()
    {
        start = false;
        lives = -1;
        Patient prev_patient = patients[patientIndex];
        Timing.KillCoroutines(prev_patient.gameObject);
        GameObject go = prev_patient.gameObject;
        patientIndex = -1;

        Sequence.Create()
            .Group(Tween.PositionY(go.transform, endValue: -10, duration: .5f, ease: Ease.InCubic).OnComplete(() => Destroy(go)))
            .Group(Tween.Scale(ui_title, endValue: Vector2.one, duration: .5f, ease: Ease.OutBack, startDelay: .2f))
            .Group(Tween.UIAnchoredPositionY(ui_icons, endValue: -800, duration: .5f, ease: Ease.InBack, startDelay: .2f))
            .Group(Tween.UIAnchoredPositionY(ui_x, endValue: -800, duration: .5f, ease: Ease.InBack, startDelay: .2f))
            .OnComplete(() => Init());
    }

    void LoadWin()
    {
        win = true;
        Patient prev_patient = patients[patientIndex];
        Timing.KillCoroutines(prev_patient.gameObject);
        GameObject go = prev_patient.gameObject;

        Sequence.Create()
            .Group(Tween.PositionY(go.transform, endValue: -10, duration: .5f, ease: Ease.InCubic, startDelay: .5f))//.OnComplete(() => Destroy(go)))
            .Group(Tween.Scale(ui_you_win, endValue: Vector2.one, duration: .5f, ease: Ease.OutBack, startDelay: 1f)).ChainCallback(() => AudioPlayer.ap.PlaySfx(perfectRun ? you_win_perfect : you_win, 1))
            .Group(Tween.Scale(ui_perfect, endValue: perfectRun ? Vector2.one : Vector2.zero, duration: .25f, ease: Ease.OutBack, startDelay: 1f));
    }

    void LoadNextPatient()
    {
        if (lives < 0)
            return;

        perfectPatient = true;

        if (lives == 0)
        {
            start = false;
            lives = -1;
            Patient prev_patient = patients[patientIndex];
            Timing.KillCoroutines(prev_patient.gameObject);
            GameObject go = prev_patient.gameObject;

            Sequence.Create()
                .Group(Tween.PositionY(go.transform, endValue: -10, duration: .5f, startDelay: 1f, ease: Ease.InCubic).OnComplete(() => Destroy(go)))
                .Group(Tween.Scale(ui_title, endValue: Vector2.one, duration: .5f, ease: Ease.OutBack, startDelay: 1.75f))
                .Group(Tween.UIAnchoredPositionY(ui_icons, endValue: -800, duration: .5f, ease: Ease.InBack, startDelay: 1.75f))
                .Group(Tween.UIAnchoredPositionY(ui_x, endValue: -800, duration: .5f, ease: Ease.InBack, startDelay: 1.75f))
                .OnComplete(() => Init());

            patientIndex = -1;
            return;
        }

        if (patientIndex >= 0)
        {
            Patient prev_patient = patients[patientIndex];

            Timing.KillCoroutines(prev_patient.gameObject);
            GameObject go = prev_patient.gameObject;

            if (prev_patient.lives > 0)
            {
                Tween.PositionX(go.transform, endValue: -20, duration: .25f, ease: Ease.InCirc)
                .OnComplete(() => Destroy(go));
            }
            else
            {
                Tween.PositionY(go.transform, endValue: -10, duration: .25f, ease: Ease.InCirc)
                .OnComplete(() => Destroy(go));
            }
        }
        if (patientIndex == 0 && patients[patientIndex].lives <= 0)
        {
            patients[patientIndex].Reset();
            patientIndex--;
        }

        patientIndex++;

        if (patientIndex >= patientSprites.Count)
            return;

        patients[patientIndex].Load(patientSprites[patientIndex]);

        if (patientIndex == 0)
        {
            Sequence.Create()
                .Group(Tween.Scale(ui_title, endValue: Vector2.zero, duration: .5f, ease: Ease.InBack))
                .Group(Tween.UIAnchoredPositionY(ui_icons, endValue: -600, duration: .4f, ease: Ease.OutBack, startDelay: .5f))
                .Group(Tween.UIAnchoredPositionY(ui_x, endValue: -600, duration: .4f, ease: Ease.OutBack, startDelay: .5f))
            ;
        }
        Tween.PositionX(patients[patientIndex].gameObject.transform, endValue: 0, duration: .75f, ease: Ease.OutBack, startDelay: patientIndex > 0 ? .25f : .75f);
    }

    // ------------------------------------------------- Title Click
    public void ClickTitle()
    {
        if (!start)
            return;

        if (patientIndex < patients.Count - 1)
        {
            LoadNextPatient();
        }
    }

    public void EnterTitle()
    {
        if (!start)
            return;

        ui_title_text.color = Color.white;
    }

    public void ExitTitle()
    {
        if (!start)
            return;

        ui_title_text.color = Color.black;
    }

    public void ClickWin()
    {
        if (win_click)
            return;

        win_click = true;

        Sequence.Create()
            .Group(Tween.Scale(ui_you_win, endValue: Vector2.zero, duration: .25f, ease: Ease.InBack))
            .Group(Tween.Scale(ui_perfect, endValue: Vector2.zero, duration: .25f, ease: Ease.InBack));

        Esc();
    }

    public void EnterWin()
    {
        if (win_click)
            return;

        ui_you_win.GetComponent<TextMeshProUGUI>().color = Color.white;
    }

    public void ExitWin()
    {
        if (win_click)
            return;

        ui_you_win.GetComponent<TextMeshProUGUI>().color = Color.black;
    }
}
