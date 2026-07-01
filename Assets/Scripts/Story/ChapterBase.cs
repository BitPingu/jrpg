using System.Collections.Generic;
using UnityEngine;


public class ChapterBase : MonoBehaviour
{
    public List<EventBase> Events;
    public EventBase CurrentEvent;
    public int EventIndex = 0;

    public virtual void BeginChapter()
    {
        // start at first event
        if (Events.Count > 0 && Events[EventIndex] != null)
        {
            CurrentEvent = Instantiate(Events[EventIndex], transform);
            SetupEvent();
        }
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
        Destroy(CurrentEvent.gameObject); // end current event
        CurrentEvent = Instantiate(Events[++EventIndex], transform); // next event
        SetupEvent(); // setup
    }

    public virtual bool Advance() { return false; }
}
