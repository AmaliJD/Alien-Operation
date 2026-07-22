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

    void Awake()
    {
        NewPatient().AddAilments(
            new Ailment(5, 1, true, -3),
            new Ailment(8, 2, false, 3)
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
        
        Patient patient = patients[patientIndex];
        if (!patient.finished)
        {
            if (patient.IsCurrentFinished())
            {
                patient.LoadNextAilments();
            }

            foreach (Ailment ail in patient.currentAilments)
            {
                ail.DecrementTime();
                GLGizmos.DrawText(ail.displayTime.ToString(), ail.location, null, 10);
            }
        }
    }

    void LoadNextPatient()
    {
        patientIndex++;
    }
}
