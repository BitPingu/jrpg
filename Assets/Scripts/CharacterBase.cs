using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterBase : MonoBehaviour
{
    public Rigidbody2D RB { get; set; }
    public Animator Anim { get; set; }

    [SerializeField] private float _moveSpeed = 3f;

    public List<Ability> Abilities;

    public StateMachine StateMachine { get; set; }
    public StateBase IdleState { get; set; }
    public StateBase BattleState { get; set; }

    public CharacterBase Opponent { get; set; }
    public bool IsAttacking { get; set; }

    [SerializeField] private float _maxHealth = 3f;
    private float _currentHealth; 

    private Coroutine _damageFlashCoroutine;


    protected virtual void Awake()
    {
        // set components
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        // set parameters
        _currentHealth = _maxHealth;

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
        RB.linearVelocity = inputVector * _moveSpeed;

        // animate
        Anim.SetFloat("Movement", RB.linearVelocity.magnitude / _moveSpeed);
    }

    public void FaceOpponent(CharacterBase opponent)
    {
        if (opponent.transform.position.x > transform.position.x)
            GetComponent<SpriteRenderer>().flipX = false;
        else if (opponent.transform.position.x < transform.position.x)
            GetComponent<SpriteRenderer>().flipX = true;
    }

    protected IEnumerator Attack()
    {
        Vector3 attackDir = (Opponent.transform.position - transform.position).normalized;
        Vector3 attackPos = Opponent.transform.position - (attackDir*1.5f);

        IsAttacking = true;

        // go to enemy
        float _distance = Vector2.Distance(Opponent.transform.position, transform.position);
        while (_distance > 0.7f)
        {
            _distance = Vector2.Distance(Opponent.transform.position, transform.position);
            Move(Opponent.transform.position - transform.position);
            yield return new WaitForFixedUpdate();
        }

        Anim.SetTrigger("Attack");
        yield return new WaitForSeconds(.1f);

        // attack
        if (Opponent.Opponent == null)
            Opponent.Opponent = this;
        Opponent.Damage(1f);

        // return to pos
        _distance = Vector2.Distance(attackPos, transform.position);
        while (_distance > 0.1f)
        {
            _distance = Vector2.Distance(attackPos, transform.position);
            Move(attackPos - transform.position);
            yield return new WaitForFixedUpdate();
        }

        IsAttacking = false;
    }

    public void Damage(float damageAmount)
    {
        // take damage
        _currentHealth -= damageAmount;
        Debug.Log(name + " took " + damageAmount + " damage.");

        if (_currentHealth <= 0)
        {
            Debug.Log(name + " died!");
        }

        // damage flash
        CallDamageFlash();

        // update health bar
        GetComponentInChildren<HealthBar>().UpdateHealthBar(_maxHealth, _currentHealth);
    }

    public void CallDamageFlash()
    {
        _damageFlashCoroutine = StartCoroutine(DamageFlash());
    }

    private IEnumerator DamageFlash()
    {
        // set color
        GetComponent<SpriteRenderer>().material.SetColor("_FlashColor", Color.white);
        float flashTime = .25f;

        float currentFlashAmount;
        float elapsedTime = 0f;
        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;
            // lerp flash amount
            currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / flashTime));
            // set flash amount
            GetComponent<SpriteRenderer>().material.SetFloat("_FlashAmount", currentFlashAmount);
            yield return null;
        }
    }

}

