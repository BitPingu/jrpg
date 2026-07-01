using System.Collections.Generic;
using UnityEngine;


public class ChapterBase : MonoBehaviour
{
    public List<EventBase> Events;
    public EventBase CurrentEvent;
    public int EventIndex = -1;

    public virtual void BeginChapter()
    {
        // start at first event
        NextEvent();
    }

    public virtual void EndChapter()
    {
        Debug.Log("end chapter " + CurrentEvent);
    }

    public virtual void Active()
    {
        if (CurrentEvent && CurrentEvent.EventIsDone)
        {
            NextEvent();
        }
    }

    public virtual void SetupEvent() { }

    public virtual void NextEvent()
    {
        if (CurrentEvent != null)
            Destroy(CurrentEvent.gameObject); // end current event

        ++EventIndex;

        if (Events.Count > 0 && EventIndex < Events.Count && Events[EventIndex] != null)
        {
            CurrentEvent = Instantiate(Events[EventIndex], transform); // next event
            SetupEvent(); // setup
        }
        else
        {
            // end chapter after last event
            CurrentEvent = null;
            EndChapter();
        }
    }

    public virtual bool Advance() { return false; }
}
