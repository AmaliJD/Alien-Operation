using GLDebug;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Main : MonoBehaviour
{
    List<Patient> patients = new();
    int patientIndex = 0;

    const float AIL_RADIUS = 1;
    bool readMouseDown;

    public TMP_FontAsset ref_activeFont;
    public TMP_FontAsset ref_fadedFont;
    public TMP_FontAsset ref_successFont;
    public GameObject ref_ailmentGameObject;

    void Awake()
    {
        Fonts.activeFont = ref_activeFont;
        Fonts.fadedFont = ref_fadedFont;
        Fonts.successFont = ref_successFont;
        Ref.ailmentGameObject = ref_ailmentGameObject;

        NewPatient().AddAilments(
            new Ailment(5, 1, false, -1, -3),
            new Ailment(8, 1.2f, false, 2, 3),
            new Ailment(5, 1.2f, true, 1, -3, 3),
            new Ailment(12, 1.5f, false, 1, 3, 3)
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
            if (patientIndex < patients.Count)
            {
                LoadNextPatient();
            }
        }

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
                bool cursorOnAil = Vector2.Distance(mousePos, ail.location) <= AIL_RADIUS;

                if (ail.complete)
                    return;

                ail.DecrementTime();
                //ail.UpdateDisplayText();
                //GLGizmos.SetColor(ail.complete ? Color.red : (cursorOnAil ? Color.yellow : Color.white));
                //GLGizmos.DrawText(ail.displayTime.ToString(), ail.location, null, 10);
                //GLGizmos.DrawOpenCircle(ail.location, AIL_RADIUS);

                if (ail.AtFadeTime() && ail.state == AilmentState.CountDown)
                {
                    ail.state = AilmentState.Faded;
                    ail.fadedDisplayTime = ail.displayTime;
                }
                if (ail.displayTime < 0)
                {
                    ail.state = AilmentState.Fail_Late;
                }
                else if (mousePressed && cursorOnAil && readMouseDown)
                {
                    if (ail.displayTime == 0)
                        ail.state = AilmentState.Success;
                    else
                        ail.state = AilmentState.Fail_Early;
                }

                ail.UpdateDisplayText();
            }
        }
    }

    void LoadNextPatient()
    {
        patientIndex++;
    }
}
