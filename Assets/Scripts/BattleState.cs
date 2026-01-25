using UnityEngine;
using UnityEngine.UI;

public class BattleState : StateBase
{
    // pass in any parameters you need in the constructors
    public BattleState(CharacterBase character, StateMachine stateMachine) : base(character, stateMachine)
    {

    }

    // code that runs when we first enter the state
    public override void EnterState()
    {
        base.EnterState();

        if (character.GetComponent<Player>() || character.GetComponent<Companion>())
            character.Anim.SetLayerWeight(1, 1);

        character.HBar.gameObject.GetComponent<Image>().enabled = true;

        // Player goes first
        // TODO: compare speed with enemy
        // if faster, can attack again (first)? else enemy attacks first
        character.BattleTurn = false;
        if (character.GetComponent<Player>())
            character.BattleTurn = true;

        Debug.Log(character.name + " engages " + character.Opponent.name + ".");
    }

    // code that runs when we exit the state
    public override void ExitState()
    {
        base.ExitState();

        if (character.GetComponent<Player>() || character.GetComponent<Companion>())
            character.Anim.SetLayerWeight(1, 0);

        character.HBar.gameObject.GetComponent<Image>().enabled = false;

        // Experience
        if (character.GetComponent<Player>() || character.GetComponent<Companion>())
            character.GainExperience(40);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        // battle
        if (character.Opponent != null)
            character.Battle();

        // exit battle
        if (character.Opponent == null)
            character.StateMachine.ChangeState(character.IdleState);

        // death
        if (character.CurrentHealth <= 0)
            character.StateMachine.End();
    }
}



