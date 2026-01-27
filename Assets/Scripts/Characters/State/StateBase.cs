
public class StateBase
{
    protected CharacterBase character;
    protected StateMachine stateMachine;


    public StateBase(CharacterBase character, StateMachine stateMachine)
    {
        this.character = character;
        this.stateMachine = stateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }
}

