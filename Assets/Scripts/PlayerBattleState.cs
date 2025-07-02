using UnityEngine;

public class PlayerBattleState : StateBase
{
    // pass in any parameters you need in the constructors
    public PlayerBattleState(CharacterBase character, StateMachine stateMachine) : base(character, stateMachine)
    {

    }

    // code that runs when we first enter the state
    public override void EnterState()
    {
        base.EnterState();

        character.Anim.SetLayerWeight(1, 1);

        Debug.Log(character.name + " is attacking " + character.Opponent.name + ".");
    }

    // code that runs when we exit the state
    public override void ExitState()
    {
        base.ExitState();

        character.Anim.SetLayerWeight(1, 0);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        // Debug.Log(character.name + " attacking...");

        // allow battling
        if (character.Opponent != null)
            character.Battle();

        // exit battle
        if (character.Opponent == null)
            character.StateMachine.ChangeState(character.IdleState);

        // player dies
        // if (character.CurrentHealth <= 0f)
        //     character.StateMachine.End();
    }
}



