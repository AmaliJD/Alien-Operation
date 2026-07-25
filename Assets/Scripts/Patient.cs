using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Patient
{
    public List<Ailment> ailments = new();
    public List<Ailment> currentAilments = new();
    int ailmentIndex = -1;

    public SpriteRenderer body;
    public SpriteRenderer face;
    public SpriteRenderer body_hit;
    public SpriteRenderer face_hit;
    PatientSprites sprites;

    public bool started;
    public bool healed;
    public bool finished;

    public GameObject gameObject;

    public void AddAilments(params Ailment[] ailments)
    {
        foreach (Ailment ail in ailments)
        {
            this.ailments.Add(ail);
            ail.patient = this;
        }
    }

    public bool IsCurrentFinished()
    {
        if (currentAilments.Count == 0)
            return true;

        bool allAilmentsDone = true;

        foreach (Ailment ail in currentAilments)
        {
            if (!ail.complete)
            {
                allAilmentsDone = false;
                break;
            }
        }

        return allAilmentsDone;
    }

    public void LoadNextAilments()
    {
        //foreach (Ailment ail in currentAilments)
        //    ail.ailmentGameObject.SetActive(false);

        currentAilments.Clear();

        for (int i = ailmentIndex; i < ailments.Count; i++)
        {
            ailmentIndex++;
            if (ailmentIndex == ailments.Count)
                return;

            currentAilments.Add(ailments[ailmentIndex]);
            ailments[ailmentIndex].InitGameObject();

            if (!ailments[ailmentIndex].loadNext)
                break;
        }
    }

    public void Load(PatientSprites sprites)
    {
        gameObject = GameObject.Instantiate(Ref.patientGameObject, new Vector2(20, .25f), Quaternion.identity);
        body = gameObject.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        face = gameObject.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();
        body_hit = gameObject.transform.GetChild(0).GetChild(2).GetComponent<SpriteRenderer>();
        face_hit = gameObject.transform.GetChild(0).GetChild(3).GetComponent<SpriteRenderer>();

        if (sprites.body != null)
        {
            body.sprite = sprites.body;
            body_hit.sprite = sprites.body;
        }

        if (sprites.faces.Count > 0)
        {
            face.sprite = sprites.faces[0];
            face_hit.sprite = sprites.faces[0];
        }

        this.sprites = sprites;
    }

    public IEnumerator<float> _SwapFaces(int i)
    {
        if (sprites.faces.Count <= i)
            yield break;

        face.sprite = sprites.faces[i];
        face_hit.sprite = sprites.faces[i];

        float time = 0;
        while (time < 1)
        {
            yield return Timing.WaitForOneFrame;
            time += Time.deltaTime;
        }

        face.sprite = sprites.faces[0];
        face_hit.sprite = sprites.faces[0];
    }

    public void SetFace(int i)
    {
        if (sprites.faces.Count <= i)
            return;

        face.sprite = sprites.faces[i];
        face_hit.sprite = sprites.faces[i];
    }


}
