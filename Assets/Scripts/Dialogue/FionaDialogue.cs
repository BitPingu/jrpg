using UnityEngine;

public class FionaDialogue : Talk
{
    public override void EndDialogue()
    {
        // call base class
        base.EndDialogue();

        GetComponentInParent<Companion>().Join(_player);
    }
}
