using System.Collections.Generic;
using UnityEngine;


public class StoryBase : MonoBehaviour
{
    public virtual void BeginStory() { }
    public virtual void Active() { }
    public virtual bool Advance() { return false; }
}
