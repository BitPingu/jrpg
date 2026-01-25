using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CharacterBase : MonoBehaviour
{
    public Rigidbody2D RB { get; set; }
    public Animator Anim { get; set; }

    public StateMachine StateMachine { get; set; }
    public StateBase IdleState { get; set; }
    public StateBase BattleState { get; set; }

    public CharacterBase Opponent { get; set; }

    [SerializeField] private float _moveSpeed = 3f;

    public int Level = 1;
    [SerializeField] private int _maxExp = 100;
    private int _currentExp = 0;

    [SerializeField] private float _maxHealth = 3f;
    public float CurrentHealth { get; set; }

    [SerializeField] private float _attack = 1f;

    public List<Ability> Abilities;

    private Coroutine _damageFlashCoroutine;

    public bool BattleTurn { get; set; }

    public Bar HBar, EBar;


    protected virtual void Awake()
    {
        // set components
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        // set parameters
        CurrentHealth = _maxHealth;

        // state machine
        StateMachine = new StateMachine();

        // states
        IdleState = new IdleState(this, StateMachine);
        BattleState = new BattleState(this, StateMachine);
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
        FaceCharacter(Opponent);
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

    public void FaceCharacter(CharacterBase character)
    {
        if (character.transform.position.x > transform.position.x)
            GetComponent<SpriteRenderer>().flipX = false;
        else if (character.transform.position.x < transform.position.x)
            GetComponent<SpriteRenderer>().flipX = true;
    }

    protected IEnumerator CallAttack()
    {
        Debug.Log(name + " is attacking " + Opponent.name + ".");
        StartCoroutine(Attack());
        yield return new WaitForSeconds(2f);
        // opponent turn
        Opponent.BattleTurn = true;
    }

    protected IEnumerator Attack()
    {
        Vector3 attackDir = (Opponent.transform.position - transform.position).normalized;
        Vector3 attackPos = Opponent.transform.position - (attackDir*1.5f);

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
        Opponent.Damage(_attack);

        // return to pos
        _distance = Vector2.Distance(attackPos, transform.position);
        while (_distance > 0.1f)
        {
            _distance = Vector2.Distance(attackPos, transform.position);
            Move(attackPos - transform.position);
            yield return new WaitForFixedUpdate();
        }

        Move(Vector2.zero);
    }

    public void Damage(float damageAmount)
    {
        // take damage
        CurrentHealth -= damageAmount;
        Debug.Log(name + " took " + damageAmount + " damage.");

        if (CurrentHealth <= 0)
        {
            Debug.Log(name + " died!");
            StartCoroutine(Die());
        }
        else
        {
            // damage flash
            CallDamageFlash();
        }

        // update health bar
        HBar.UpdateBar(_maxHealth, CurrentHealth);
    }

    private IEnumerator Die()
    {
        // set color
        GetComponent<SpriteRenderer>().material.SetFloat("_FlashAmount", 1f);
        float flashTime = 2f;

        float currentFlashAmount;
        float elapsedTime = 0f;
        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;
            // lerp flash amount
            currentFlashAmount = Mathf.Lerp(0f, 1f, (elapsedTime / flashTime));
            // set flash amount
            GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", currentFlashAmount);
            yield return null;
        }

        // Die
        Destroy(gameObject);
    }

    public void CallDamageFlash()
    {
        _damageFlashCoroutine = StartCoroutine(DamageFlash());
    }

    private IEnumerator DamageFlash()
    {
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

    public void GainExperience(int amount)
    {
        _currentExp += amount;
        Debug.Log(name + " gained " + amount + " exp.");

        StartCoroutine(ShowEBar());

        if (_currentExp >= _maxExp)
        {
            StartCoroutine(LevelUp());
        }
        else
        {
            Debug.Log("Until next level: " + (_maxExp-_currentExp) + " exp");
        }
    }

    private IEnumerator ShowEBar()
    {
        EBar.gameObject.GetComponent<Image>().enabled = true;

        // update health bar
        EBar.UpdateBar(_maxExp, _currentExp);
        yield return new WaitForSeconds(2f);

        EBar.gameObject.GetComponent<Image>().enabled = false;
    }

    private IEnumerator LevelUp()
    {
        // increase level
        Level++;

        // update stats
        _maxHealth = Level * 40;
        CurrentHealth = _maxHealth;
        HBar.UpdateBar(_maxHealth, CurrentHealth);
        _attack = Level * 3 + 3;

        // reset exp
        _currentExp = 0;
        _maxExp = Level * 100;

        yield return new WaitForSeconds(.3f);

        // glow up
        Debug.Log(name + " grew to level " + Level + "!");
        GetComponent<SpriteRenderer>().material.SetColor("_FlashColor", new Color(0, 243, 255));
        CallDamageFlash();
        yield return new WaitForSeconds(1f);
        GetComponent<SpriteRenderer>().material.SetColor("_FlashColor", Color.white);

        EBar.UpdateBar(_maxExp, _currentExp);
    }

}

