using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public Rigidbody2D RB { get; set; }
    public Animator Anim { get; set; }

    [field: SerializeField] public float MaxSpeed { get; set; }
    [field: SerializeField] public float MoveSpeed { get; set; }


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
    }

    protected virtual void Update()
    {

    }

    protected virtual void Move(Vector3 inputVector)
    {
        // normalize
        inputVector = inputVector.normalized;

        // face direction
        if (inputVector.x > 0)
            GetComponent<SpriteRenderer>().flipX = false;
        else if (inputVector.x < 0)
            GetComponent<SpriteRenderer>().flipX = true;

        // move
            RB.linearVelocity = inputVector * MoveSpeed;

        // animate
        Anim.SetFloat("Movement", RB.linearVelocity.magnitude/MoveSpeed);
    }
}

