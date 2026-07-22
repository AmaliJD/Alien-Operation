using System.Collections.Generic;
using UnityEngine;

public class Patient
{
    public List<Ailment> ailments = new();
    public List<Ailment> currentAilments = new();
    int ailmentIndex = -1;

    public bool started;
    public bool healed;
    public bool finished;

    public void AddAilments(params Ailment[] ailments)
    {
        foreach (Ailment ail in ailments)
        {
            this.ailments.Add(ail);
        }
    }

    public bool IsCurrentFinished()
    {
        if (currentAilments.Count == 0)
            return true;

        bool allAilmentsDone = true;

        foreach (Ailment ail in currentAilments)
        {
            if (!ail.done)
            {
                allAilmentsDone = false;
                break;
            }
        }

        return allAilmentsDone;
    }

    public void LoadNextAilments()
    {
        currentAilments.Clear();

        for (int i = ailmentIndex; i < ailments.Count; i++)
        {
            ailmentIndex++;
            if (ailmentIndex == ailments.Count)
                return;

            currentAilments.Add(ailments[ailmentIndex]);

            if (!ailments[ailmentIndex].loadNext)
                break;
        }
    }
}
