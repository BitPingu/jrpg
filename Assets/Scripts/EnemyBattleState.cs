using UnityEngine;
using UnityEngine.UI;

public class EnemyBattleState : StateBase
{
    // pass in any parameters you need in the constructors
    public EnemyBattleState(CharacterBase character, StateMachine stateMachine) : base(character, stateMachine)
    {

    }

    // code that runs when we first enter the state
    public override void EnterState()
    {
        base.EnterState();

        character.GetComponentInChildren<HealthBar>().gameObject.GetComponent<Image>().enabled = true;

        Debug.Log(character.name + " is attacking " + character.Opponent.name + ".");
    }

    // code that runs when we exit the state
    public override void ExitState()
    {
        base.ExitState();

        character.GetComponentInChildren<HealthBar>().gameObject.GetComponent<Image>().enabled = false;
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

        // enemy dies
        // if (character.CurrentHealth <= 0f)
        //     character.StateMachine.End();
    }
}



