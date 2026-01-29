using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StoryBase : MonoBehaviour
{
    public List<EventBase> Events;
    public EventBase CurrentEvent;
    public int EventIndex;

    public virtual void BeginStory()
    {
        // start at first event
        EventIndex = 0;
    }

    public virtual void Active() { }
    public virtual IEnumerator NextEvent() { yield return null; }
    public virtual bool Advance() { return false; }
}
