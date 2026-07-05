using UnityEngine.UI;

public class BattleState : StateBase
{
    private FighterBase _fighter;

    // pass in any parameters you need in the constructors
    public BattleState(FighterBase fighter, StateMachine stateMachine) : base(fighter, stateMachine)
    {
        _fighter = fighter;
        this.stateMachine = stateMachine;
    }

    // code that runs when we first enter the state
    public override void EnterState()
    {
        base.EnterState();

        if (_fighter.GetComponent<Player>())
        {
            // camera
            CameraController.Instance.follow = false;
        }

        // battle anim
        if (_fighter.GetComponent<PartyBase>())
            _fighter.Anim.SetLayerWeight(1, 1);

        // show health bar
        _fighter.HBar.gameObject.GetComponent<Image>().enabled = true;

        if (_fighter.GetComponent<Player>() == null)
            _fighter.BattleTurn = false;
        _fighter.WinBattle = false;
    }

    // code that runs when we exit the state
    public override void ExitState()
    {
        base.ExitState();

        if (_fighter.GetComponent<Player>())
        {
            // camera
            CameraController.Instance.follow = true;
        }

        // battle anim
        if (_fighter.GetComponent<PartyBase>())
            _fighter.Anim.SetLayerWeight(1, 0);

        // hide health bar
        _fighter.HBar.gameObject.GetComponent<Image>().enabled = false;

        if (_fighter.GetComponent<PartyBase>())
        {
            _fighter.GetComponent<PartyBase>().BattleHUD.SetActive(false);
        }
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        // battle
        if (_fighter.Opponent != null)
            _fighter.Battle();

        // exit battle
        if (_fighter.Opponent == null)
            character.StateMachine.ChangeState(character.IdleState);

        // death
        if (_fighter.CurrentHealth <= 0)
            character.StateMachine.End();
    }
}

