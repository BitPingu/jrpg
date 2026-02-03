using UnityEngine;

public class Villager : CharacterBase
{
    // public bool TalkedTo { get; set; }

    public override void Idle()
    {
        // call base class 
        base.Idle();

        if (Anim)
            Move(Vector2.zero);
    }
}
