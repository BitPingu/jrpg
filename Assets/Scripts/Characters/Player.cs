using System.Collections;
using UnityEngine;

public class Player : PartyBase
{
    public Companion CurrentCompanion { get; set; }

    public bool CanEnter { get; set; }
    public bool Entered { get; set; }

    protected override void Update()
    {
        // call base class
        base.Update();
    }

    public override void Idle()
    {
        // call base class
        base.Idle();

        // allow player movement
        Move(new Vector2(Input.HorizontalInput, Input.VerticalInput));
    }

    public override void Battle()
    {
        // call base class
        base.Battle();

        // player only battle options go here
    }

    public IEnumerator Engage(Enemy enemy)
    {
        Opponent = enemy;

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

        // companion engage
        if (CurrentCompanion)
        {
            CurrentCompanion.Opponent = Opponent;
            // GetComponent<Player>().Elf.CallBattlePos(attackPos);
        }

        Opponent.Opponent = this;
        Opponent.Damage(0f);

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

    protected override IEnumerator CallAttack()
    {
        Debug.Log(name + " is attacking " + Opponent.name + ".");
        StartCoroutine(Attack());

        yield return new WaitForSeconds(2f);

        if (CurrentCompanion && !WinBattle)
        {
            // companion turn
            CurrentCompanion.BattleTurn = true;
        }
        else
        {
            // enemy turn
            Opponent.BattleTurn = true;
        }
    }

    protected override void Run()
    {
        // call base class
        base.Run();

        if (CurrentCompanion)
        {
            CurrentCompanion.Opponent = null;
        }
    }
}

