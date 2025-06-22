using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public Rigidbody2D RB { get; set; }
    public Animator Anim { get; set; }

    [field: SerializeField] public float MaxSpeed { get; set; }
    [field: SerializeField] public float MoveSpeed { get; set; }

    public StateMachine StateMachine { get; set; }
    public StateBase IdleState { get; set; }
    public StateBase BattleState { get; set; }

    public CharacterBase Opponent { get; set; }


    protected virtual void Awake()
    {
        // set components
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        // set parameters
        MoveSpeed = MaxSpeed;

        // state machine
        StateMachine = new StateMachine();
    }

    protected virtual void Update()
    {
        // run current state
        if (StateMachine.CurrentState != null)
            StateMachine.CurrentState.FrameUpdate();
    }

    public virtual void Idle()
    {

    }

    public virtual void Battle()
    {
        // face opponent
        FaceOpponent(Opponent);
    }

    protected virtual void Move(Vector2 inputVector)
    {
        // normalize
        inputVector = inputVector.normalized;

        // face direction (when idle)
        if (StateMachine.CurrentState == IdleState)
        {
            if (inputVector.x > 0)
                GetComponent<SpriteRenderer>().flipX = false;
            else if (inputVector.x < 0)
                GetComponent<SpriteRenderer>().flipX = true;
        }

        // move
        RB.linearVelocity = inputVector * MoveSpeed;

        // animate
        Anim.SetFloat("Movement", RB.linearVelocity.magnitude / MoveSpeed);
    }

    public void FaceOpponent(CharacterBase opponent)
    {
        if (opponent.transform.position.x > transform.position.x)
            GetComponent<SpriteRenderer>().flipX = false;
        else if (opponent.transform.position.x < transform.position.x)
            GetComponent<SpriteRenderer>().flipX = true;
    }
}

