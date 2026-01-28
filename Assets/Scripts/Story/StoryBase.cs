using UnityEngine;


public class StoryBase : MonoBehaviour
{
    protected string Name;
    public virtual void BeginStory() { }
    public virtual void Active() { }
    public virtual bool Advance() { return false; }
}
