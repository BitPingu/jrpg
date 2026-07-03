using System.Collections;
using UnityEngine;

public class Companion : PartyBase
{
    public Player Leader { get; set; }

    [SerializeField] private float _followDistance = 1f;

    public bool IsSparring { get; set; }

    public override void Idle()
    {
        // call base class
        base.Idle();

        if (Leader)
        {
            Follow();
        }
        else
        {
            Move(Vector2.zero);
        }
    }

    public void Join(Player player)
    {
        player.CurrentCompanion = this;
        Leader = player;
        Debug.Log(name + " joins the party!");
    }

    private void Follow()
    {
        float distance = Vector2.Distance(Leader.transform.position, transform.position);
        if (distance > _followDistance)
            Move(Leader.transform.position - transform.position);
        // Stop moving when within range
        if (distance < _followDistance-.2f)
            Move(Vector2.zero);
    }

    public void CallBattlePos(Vector3 playerAttackPos)
    {
        StartCoroutine(BattlePos(playerAttackPos));
    }

    private IEnumerator BattlePos(Vector3 playerAttackPos)
    {
        Vector3 attackDir = (Opponent.transform.position - playerAttackPos).normalized;
        Vector3 attackPos = (Opponent.transform.position - (attackDir*2.5f)) + Leader.transform.up;

        // go to pos
        float _distance = Vector2.Distance(attackPos, transform.position);
        while (_distance > 0.1f)
        {
            _distance = Vector2.Distance(attackPos, transform.position);
            Move(attackPos - transform.position);
            yield return new WaitForFixedUpdate();
        }

        Move(Vector2.zero);
    }

    public override void Battle()
    {
        // attack
        if (BattleTurn)
        {
            if (!IsSparring)
            {
                // call base class
                base.Battle();
            }
            else
            {
                // act as enemy
                StartCoroutine(CallAttack());
                BattleTurn = false;
            }
        }
    }

    public override void Damage(int damageAmount)
    {
        // call base class
        base.Damage(damageAmount);

        if (CurrentHealth <= 0 && IsSparring)
        {
            StartCoroutine(Die());
        }
    }

    public override IEnumerator Die()
    {
        yield return new WaitForSeconds(2f);
        Anim.Rebind();
        Anim.enabled = false;

        string text = charName + " was defeated!";
        DialogueController.Instance.BattleDialogue(this, text, false);

        yield return new WaitForSeconds(2f);

        // exp
        Opponent.GetComponent<Player>().GainExperience(40);

        yield return new WaitForSeconds(2f);

        // end battle dialogue
        DialogueController.Instance.EndBattleDialogue();
    }

    protected override IEnumerator Run()
    {
        // call base class
        base.Run();

        Leader.Opponent = null;

        yield return null;
    }
}

