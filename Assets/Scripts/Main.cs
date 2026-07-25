using GLDebug;
using System;
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

    [Header("UI")]
    public Transform ui_title;

    void Awake()
    {
        Fonts.activeFont = ref_activeFont;
        Fonts.fadedFont = ref_fadedFont;
        Fonts.successFont = ref_successFont;
        Ref.ailmentGameObject = ref_ailmentGameObject;
        Ref.patientGameObject = ref_patientGameObject;

        NewPatient().AddAilments(
            new Ailment(5, 1f, false, 0, new Vector2(0, 0), AIL_RADIUS * 1.3f, 45)
        );

        NewPatient().AddAilments(
            new Ailment(5, 1.667f, true, 0, new Vector2(0f, 1.7f), AIL_RADIUS * 1.3f, 90),
            new Ailment(3, 1f, false, 0, new Vector2(1.4f, -1.5f), AIL_RADIUS * 1.3f, -90)
        );

        NewPatient().AddAilments(
            new Ailment(5, 1f, false, -2, new Vector2(-2.4f, 2.4f), AIL_RADIUS * 1.3f, -90),
            new Ailment(5, 1f, false, -2, new Vector2(1.2f, 1.3f), AIL_RADIUS * 1.3f, 45),
            new Ailment(10, 2f, false, -4, new Vector2(-.3f, 0), AIL_RADIUS * 1.9f, -135)
        );

        NewPatient().AddAilments(
            new Ailment(5, 1f, false, -3, new Vector2(0f, 1.7f), AIL_RADIUS * 1.4f, 90),
            new Ailment(5, 1.667f, true, -1, new Vector2(0f, 1.7f), AIL_RADIUS * 1.3f, 90),
            new Ailment(3, 1f, false, -1, new Vector2(1.4f, -1.5f), AIL_RADIUS * 1.3f, -90)
        );

        NewPatient().AddAilments(
            new Ailment(5, 1.2f, true, 1f, new Vector2(0f, 1.7f), AIL_RADIUS * 1.4f, 90),
            new Ailment(5, .8f, false, 2f, new Vector2(0f, 1.7f), AIL_RADIUS * 1.3f, 90),
            new Ailment(3, 1f, false, -1, new Vector2(1.4f, -1.5f), AIL_RADIUS * 1.3f, -90)
        );
    }

    Patient NewPatient()
    {
        Patient p = new();
        patients.Add(p);

        return p;
    }

    void Update()
    {
        if (Keyboard.current.leftShiftKey.value == 1 && Keyboard.current.nKey.wasPressedThisFrame)
        {
            if (patientIndex < patients.Count - 1)
            {
                LoadNextPatient();
            }
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
                        Sequence.Create()
                            .Group(Tween.Scale(ail.displayText.transform, endValue: Vector2.one, duration: .25f, ease: Ease.OutBack, startDelay: patientIndex > 0 ? 1f : 1.5f))
                            .Group(Tween.Scale(ail.sprite_head.transform, endValue: Vector2.one, duration: .25f, ease: Ease.OutBack, startDelay: patientIndex > 0 ? 1f : 1.5f))
                            //.ChainDelay(.25f)
                            .OnComplete(() =>
                            {
                                ail.state = AilmentState.Ready;
                                ail.SetDisplayTime();
                            });
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

                    Timing.KillCoroutines(patient.gameObject);
                    Timing.RunCoroutine(patient._SwapFaces(1, patient.lives > 1 ? 0 : 2), patient.gameObject);

                    patient.body_hit.color = Color.red;
                    patient.face_hit.color = Color.red;
                    Sequence.Create()
                        .Group(Tween.Alpha(patient.body_hit, endValue: 0, duration: patient.lives > 1 ? .6f : 1f))
                        .Group(Tween.Alpha(patient.face_hit, endValue: 0, duration: patient.lives > 1 ? .6f : 1f))
                        .Group(Tween.ShakeLocalPosition(patient.gameObject.transform, strength: Vector2.one * (patient.lives > 1 ? .1f : .25f), duration: 1f));

                    patient.ailmentsHit++;
                    patient.lives--;
                }
                else if (mousePressed && cursorOnAil && readMouseDown)
                {
                    if (ail.displayTime == 0)
                    {
                        ail.state = AilmentState.Success;
                        Timing.KillCoroutines(ail.coroutineLayer);
                        Timing.RunCoroutine(ail._FlashHoldFadeOut(.1f, new Color(.5f, 1, 0), new Color(.9f, 1, .75f), 1f, .25f), ail.coroutineLayer);

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

                        Timing.KillCoroutines(patient.gameObject);
                        Timing.RunCoroutine(patient._SwapFaces(1, patient.lives > 1 ? 0 : 2), patient.gameObject);

                        patient.body_hit.color = Color.red;
                        patient.face_hit.color = Color.red;
                        Sequence.Create()
                            .Group(Tween.Alpha(patient.body_hit, endValue: 0, duration: patient.lives > 1 ? .6f : 1f))
                            .Group(Tween.Alpha(patient.face_hit, endValue: 0, duration: patient.lives > 1 ? .6f : 1f))
                            .Group(Tween.ShakeLocalPosition(patient.gameObject.transform, strength: Vector2.one * (patient.lives > 1 ? .1f : .25f), duration: 1f));

                        patient.ailmentsHit++;
                        patient.lives--;
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
                }
                else if (patient.lives <= 0)
                {
                    Timing.KillCoroutines(patient.gameObject);
                    patient.SetFace(3);
                }

                Timing.RunCoroutine(patient._Next(), patient.gameObject);
            }
            else if (patient.next == 2 && patientIndex < patients.Count - 1)
            {
                LoadNextPatient();
            }
        }
    }

    void LoadNextPatient()
    {
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
            Tween.Scale(ui_title, endValue: Vector2.zero, duration: .5f, ease: Ease.InBack);
        }
        Tween.PositionX(patients[patientIndex].gameObject.transform, endValue: 0, duration: .75f, ease: Ease.OutBack, startDelay: patientIndex > 0 ? .25f : .75f);
    }
}
