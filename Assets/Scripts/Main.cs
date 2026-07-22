using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using GLDebug;

public class Main : MonoBehaviour
{
    List<Patient> patients = new();
    int patientIndex = 0;

    const float AIL_RADIUS = 1;
    bool readMouseDown;

    void Awake()
    {
        NewPatient().AddAilments(
            new Ailment(5, 1, false, -3),
            new Ailment(8, 2, false, 3),
            new Ailment(5, 2, true, -3),
            new Ailment(12, 3, false, 3)
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
                ail.DecrementTime();
                GLGizmos.SetColor(ail.complete ? Color.red : (cursorOnAil ? Color.yellow : Color.white));
                GLGizmos.DrawText(ail.displayTime.ToString(), ail.location, null, 10);
                GLGizmos.DrawOpenCircle(ail.location, AIL_RADIUS);

                if (ail.displayTime < 0)
                {
                    ail.state = AilmentState.Fail;
                }
                if (!ail.complete && mousePressed && cursorOnAil && readMouseDown)
                {
                    if (ail.displayTime == 0)
                        ail.state = AilmentState.Success;
                    else
                        ail.state = AilmentState.Fail;
                }
            }
        }
    }

    void LoadNextPatient()
    {
        patientIndex++;
    }
}
