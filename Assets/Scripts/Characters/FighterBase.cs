using System.Collections;
using UnityEngine;

public class FighterBase : CharacterBase
{
    // battle state
    public StateBase BattleState { get; set; }
    public FighterBase Opponent { get; set; }

    // stats
    public int Level = 1;
    public float MaxHealth = 20f;
    public float CurrentHealth { get; set; }
    public float Strength = 4f;

    // public List<Ability> Abilities;

    // conditions
    public bool BattleTurn { get; set; }
    public bool IsAttacking { get; set; }
    public bool WinBattle { get; set; }

    // UI
    public Bar HBar;

    [SerializeField] private GameObject _projectile;
    [SerializeField] private AudioClip _hitSound;

    protected override void Start()
    {
        // call base class
        base.Start();

        // set parameters
        CurrentHealth = MaxHealth;

        // states
        BattleState = new BattleState(this, StateMachine);
    }

    public virtual void Battle()
    {
        // face opponent
        Face(Opponent);

        // idle when not attacking
        if (!IsAttacking)
            Move(Vector2.zero);
    }

    protected virtual IEnumerator CallAttack()
    {
        Debug.Log(name + " is attacking " + Opponent.name + ".");
        if (_projectile)
        {
            StartCoroutine(RangedAttack());
        }
        else
        {
            StartCoroutine(Attack());
        }

        yield return new WaitForSeconds(2f);

        // opponent turn
        if (Opponent)
            Opponent.BattleTurn = true;
    }

    protected IEnumerator Attack()
    {
        Vector3 attackDir = (Opponent.transform.position - transform.position).normalized;
        Vector3 attackPos = Opponent.transform.position - (attackDir*1.5f);

        IsAttacking = true;

        // go to opponent
        float _distance = Vector2.Distance(Opponent.transform.position, transform.position);
        while (_distance > 0.7f)
        {
            _distance = Vector2.Distance(Opponent.transform.position, transform.position);
            Move(Opponent.transform.position - transform.position);
            yield return new WaitForFixedUpdate();
        }

        Anim.SetTrigger("Attack");
        yield return new WaitForSeconds(.1f);

        // damage
        Opponent.Damage(Strength);

        // return to pos
        _distance = Vector2.Distance(attackPos, transform.position);
        while (_distance > 0.1f)
        {
            _distance = Vector2.Distance(attackPos, transform.position);
            Move(attackPos - transform.position);
            yield return new WaitForFixedUpdate();
        }

        Move(Vector2.zero);

        IsAttacking = false;
    }

    protected IEnumerator RangedAttack()
    {
        Anim.SetTrigger("Attack");

        // summon projectile 
        Vector3 projPos = transform.position+transform.up;
        GameObject proj = Instantiate(_projectile, projPos, Quaternion.identity);

        // launch projectile at opponent
        Vector3 opPos = Opponent.transform.position+(Opponent.transform.up/2f);
        float _distance = Vector2.Distance(opPos, proj.transform.position);
        while (_distance > 0.1f)
        {
            _distance = Vector2.Distance(opPos, proj.transform.position);
            Vector2 projVector = (opPos - proj.transform.position).normalized;
            // move projectile
            proj.GetComponent<Rigidbody2D>().linearVelocity = projVector * 6f;
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(.01f);

        Destroy(proj);

        yield return new WaitForSeconds(.1f);

        // damage
        Opponent.Damage(Strength);
    }

    public virtual void Damage(float damageAmount)
    {
        // take damage
        CurrentHealth -= damageAmount;
        Debug.Log(name + " took " + damageAmount + " damage.");

        if (CurrentHealth > 0)
        {
            // damage flash
            CallDamageFlash();
        }

        // damage sound
        SFXManager.Play(_hitSound);

        // update health bar
        HBar.UpdateBar(MaxHealth, CurrentHealth);
    }

    public void CallDamageFlash()
    {
        StartCoroutine(DamageFlash());
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
            currentFlashAmount = Mathf.Lerp(1f, 0f, elapsedTime / flashTime);
            // set flash amount
            Sprite.material.SetFloat("_FlashAmount", currentFlashAmount);
            yield return null;
        }
    }

}

