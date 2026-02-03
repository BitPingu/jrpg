using System.Collections.Generic;
using UnityEngine;


public class ChapterBase : MonoBehaviour
{
    public List<EventBase> Events;
    public EventBase CurrentEvent;
    public int EventIndex;

    public virtual void BeginStory()
    {
        // start at first event
        EventIndex = 0;
    }

    public virtual void Active()
    {
        if (CurrentEvent && CurrentEvent.EventIsDone)
        {
            NextEvent();
        }
    }

    public virtual void NextEvent()
    {
        Destroy(CurrentEvent.gameObject); // end current event
        CurrentEvent = Instantiate(Events[++EventIndex], transform); // next event
    }

    public virtual bool Advance() { return false; }
}
