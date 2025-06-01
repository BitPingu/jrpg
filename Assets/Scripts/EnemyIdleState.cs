using UnityEngine;

public class EnemyIdleState : StateBase
{
    // pass in any parameters you need in the constructors
    public EnemyIdleState(Enemy enemy, StateMachine stateMachine) : base(enemy, stateMachine)
    {
        // this.enemy = enemy;
    }

    // code that runs when we first enter the state
    public override void EnterState()
    {
        base.EnterState();

        ((Enemy)character).SetRandomDir();

        Debug.Log(character.name + " is idle.");
    }

    // code that runs when we exit the state
    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        // Debug.Log(character.name + " idling...");

        // allow idling
        ((Enemy)character).Idle();

        // engage player
        if (((Enemy)character).SeePlayer())
            character.StateMachine.ChangeState(character.BattleState);

        // enemy dies
        // if (character.CurrentHealth <= 0f)
        //     character.StateMachine.End();
    }
}

