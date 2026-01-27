using UnityEngine;

public class IdleState : StateBase
{
    // pass in any parameters you need in the constructors
    public IdleState(CharacterBase character, StateMachine stateMachine) : base(character, stateMachine)
    {
        
    }

    // code that runs when we first enter the state
    public override void EnterState()
    {
        base.EnterState();

        Debug.Log(character.name + " is idle.");
    }

    // code that runs when we exit the state
    public override void ExitState()
    {
        base.ExitState();

        // stop movement
        character.Move(Vector2.zero);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        // idle
        character.Idle();

        // battle opponent
        if (character.Opponent != null)
            character.StateMachine.ChangeState(character.BattleState);
    }
}

