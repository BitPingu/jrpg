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

        // battle anim
        if (character.GetComponent<Player>() || character.GetComponent<Companion>())
            character.Anim.SetLayerWeight(1, 1);

        // show health bar
        character.HBar.gameObject.GetComponent<Image>().enabled = true;

        character.BattleTurn = false;

        // Player goes first
        // TODO: compare speed with enemy
        // if faster, can attack again (first)? else enemy attacks first
        if (character.GetComponent<Player>())
            character.BattleTurn = true;

        character.WinBattle = false;

        Debug.Log(character.name + " engages " + character.Opponent.name + ".");
    }

    // code that runs when we exit the state
    public override void ExitState()
    {
        base.ExitState();

        // battle anim
        if (character.GetComponent<Player>() || character.GetComponent<Companion>())
            character.Anim.SetLayerWeight(1, 0);

        // hide health bar
        character.HBar.gameObject.GetComponent<Image>().enabled = false;

        if (character.GetComponent<Player>() || character.GetComponent<Companion>())
        {
            character.BattleHUD.SetActive(false);
            // exp
            if (character.WinBattle)
                character.GainExperience(40);
        }
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



