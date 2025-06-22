using UnityEngine;

public class PlayerIdleState : StateBase
{
    // pass in any parameters you need in the constructors
    public PlayerIdleState(Player player, StateMachine stateMachine) : base(player, stateMachine)
    {
        // this.player = player;
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
    }

    public override void FrameUpdate()
    {
        // TODO: simply state classes, idle and battle for enemy and player into 1
        base.FrameUpdate();

        // Debug.Log(character.name + " idling...");

        // allow idling
        character.Idle();

        // engage enemy
        if (character.Opponent != null)
            character.StateMachine.ChangeState(character.BattleState);

        // player dies
        // if (character.CurrentHealth <= 0f)
        //     character.StateMachine.End();
    }
}

