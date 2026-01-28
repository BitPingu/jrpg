using UnityEngine;


public class StoryBase : MonoBehaviour
{
    protected string Name;
    public virtual void BeginStory() { }
    protected virtual bool Advance() { return false; }
}
