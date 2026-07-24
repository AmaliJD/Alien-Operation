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

    public TMP_FontAsset ref_activeFont;
    public TMP_FontAsset ref_fadedFont;
    public TMP_FontAsset ref_successFont;
    public GameObject ref_ailmentGameObject;
    public GameObject ref_patientGameObject;

    [Header("Patient Sprites")]
    public List<PatientSprites> patientSprites;

    void Awake()
    {
        Fonts.activeFont = ref_activeFont;
        Fonts.fadedFont = ref_fadedFont;
        Fonts.successFont = ref_successFont;
        Ref.ailmentGameObject = ref_ailmentGameObject;
        Ref.patientGameObject = ref_patientGameObject;

        NewPatient().AddAilments(
            new Ailment(5, .8f, false, -2, new Vector2(-3, 0))
        );

        NewPatient().AddAilments(
            new Ailment(5, 1f, true, 4, new Vector2(-3, 0)),
            new Ailment(10, 1f, false, 4, new Vector2(3, 0))
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
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
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
                Vector2 ail_location = ail.ailmentGameObject.transform.position;//.location + (Vector2)ail.patient.patientGameObject.transform.position;
                bool cursorOnAil = Vector2.Distance(mousePos, ail_location) <= AIL_RADIUS;

                if (ail.complete)
                    continue;

                AilmentState currentAilstate = ail.state;

                ail.DecrementTime();

                Color gizmoColor = ail.state switch
                {
                    AilmentState.CountDown => (cursorOnAil ? new Color(1, .5f, 0) : Color.red),
                    AilmentState.Faded => (cursorOnAil ? new Color(1, .5f, 0) : Color.red),
                    AilmentState.Success => new Color(.2f, 1, 0),
                    _ => Color.red
                };
                gizmoColor.a = .2f;

                GLGizmos.SetColor(gizmoColor);
                GLGizmos.DrawWeightedCircle(ail_location, AIL_RADIUS, .1f, BorderType.Inside, -2);

                GLGizmos.DrawSolidCircle(ail_location, AIL_RADIUS - .2f, -2)
                    .SetColor(new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, cursorOnAil ? .05f : .01f));

                if (ail.AtFadeTime() && ail.state == AilmentState.CountDown)
                {
                    ail.state = AilmentState.Faded;
                    ail.fadedDisplayTime = ail.displayTime;
                    Timing.KillCoroutines(ail.couroutineLayer);
                    Timing.RunCoroutine(ail._FadeOut(2f), ail.couroutineLayer);
                }
                if (ail.displayTime < 0)
                {
                    ail.state = AilmentState.Fail_Late;
                    Timing.KillCoroutines(ail.couroutineLayer);
                    Timing.RunCoroutine(ail._FlashHoldFadeOut(.1f, Color.white, Color.red, .5f, 1f), ail.couroutineLayer);
                }
                else if (mousePressed && cursorOnAil && readMouseDown)
                {
                    if (ail.displayTime == 0)
                    {
                        ail.state = AilmentState.Success;
                        Timing.KillCoroutines(ail.couroutineLayer);
                        Timing.RunCoroutine(ail._FlashHoldFadeOut(.1f, new Color(.5f, 1, 0), new Color(.9f, 1, .75f), 1f, .25f), ail.couroutineLayer);
                    }
                    else
                    {
                        ail.state = AilmentState.Fail_Early;
                        Timing.KillCoroutines(ail.couroutineLayer);
                        Timing.RunCoroutine(ail._FlashHoldFadeOut(.1f, Color.white, Color.red, .5f, 1f), ail.couroutineLayer);
                    }
                }

                ail.UpdateDisplayText(currentAilstate != ail.state);
            }
        }
    }

    void LoadNextPatient()
    {
        if (patientIndex >= 0)
        {
            GameObject go = patients[patientIndex].patientGameObject;
            Tween.PositionX(go.transform, endValue: -20, duration: .25f, ease: Ease.InCirc)
                .OnComplete(() => Destroy(go));
        }

        patientIndex++;
        patients[patientIndex].Load(patientSprites[patientIndex]);

        Tween.PositionX(patients[patientIndex].patientGameObject.transform, endValue: 0, duration: .75f, ease: Ease.OutBack, startDelay: .25f);
    }
}
