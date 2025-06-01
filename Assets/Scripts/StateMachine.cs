
public class StateMachine
{
    public StateBase CurrentState { get; set; }


    // set the starting state
    public void Initialize(StateBase startingState)
    {
        CurrentState = startingState;
        CurrentState.EnterState();
    }

    // exit this state and enter another
    public void ChangeState(StateBase newState)
    {
        CurrentState.ExitState();
        CurrentState = newState;
        CurrentState.EnterState();
    }

    public void End()
    {
        CurrentState.ExitState();
        CurrentState = null;
    }
}

