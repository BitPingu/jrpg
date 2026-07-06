using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterBase : CharacterBase
{
    // battle state
    public StateBase BattleState { get; set; }
    public FighterBase Opponent { get; set; }
    public List<FighterBase> Opponents = new List<FighterBase>();

    // stats
    public int Level = 1;
    public int MaxHealth = 20;
    public int CurrentHealth { get; set; }
    public int Strength = 4;

    // public List<Ability> Abilities;

    // conditions
    public bool BattleTurn { get; set; }
    public bool IsAttacking { get; set; }
    public bool WinBattle { get; set; }

    // UI
    public Bar HBar;

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
        // battle dialogue
        string text = charName + " attacks!";
        DialogueController.Instance.BattleDialogue(this, text, false);

        StartCoroutine(Attack());

        yield return new WaitForSeconds(1.5f);

        // opponent turn
        if (Opponents[0])
            Opponents[0].BattleTurn = true;
    }

    protected virtual IEnumerator Attack()
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

    public virtual void Damage(int damageAmount)
    {
        // take damage
        CurrentHealth -= damageAmount;

        // damage flash
        CallDamageFlash();

        if (CurrentHealth <= 0)
        {
            damageAmount -= -CurrentHealth;
            CurrentHealth = 0;
        }

        // update health bar
        HBar.UpdateBar(MaxHealth, CurrentHealth);

        // battle dialogue
        string text = charName + " took " + damageAmount + " damage.";
        DialogueController.Instance.BattleDialogue(this, text, false);
        Debug.Log("Current health: " + CurrentHealth + "/" + MaxHealth);
    }

    public void CallDamageFlash()
    {
        // damage sound
        SFXManager.Play(_hitSound);
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

    public virtual IEnumerator Die()
    {
        yield return null;
    }

    public void Heal(int healAmount)
    {
        // restore health
        CurrentHealth += healAmount;

        if (CurrentHealth > MaxHealth)
        {
            healAmount -= CurrentHealth - MaxHealth;
            CurrentHealth = MaxHealth;
        }

        // update health bar
        HBar.UpdateBar(MaxHealth, CurrentHealth);

        Debug.Log(name + " restored " + healAmount + " health.");
        Debug.Log("Current health: " + CurrentHealth + "/" + MaxHealth);
    }
}

