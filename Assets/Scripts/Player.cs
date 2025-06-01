using UnityEngine;

public class Player : CharacterBase
{
    protected override void Start()
    {
        // call base class
        base.Start();
    }

    protected override void Update()
    {
        // call base class
        base.Update();

        // allow player movement
        Move(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
    }
}

