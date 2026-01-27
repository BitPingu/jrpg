using System.Collections;
using UnityEngine;

public class Player : CharacterBase
{
    public PlayerController Input { get; set; }

    public Companion Elf { get; set; }

    protected override void Start()
    {
        // call base class
        base.Start();

        // player components
        Input = GetComponent<PlayerController>();

        // start in idle state
        StateMachine.Initialize(IdleState);
    }

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

        // attack
        if (BattleTurn)
        {
            // show HUD
            BattleHUD.SetActive(true);

            // attack
            if (Input.E)
            {
                StartCoroutine(CallAttack());
                BattleTurn = false;
                BattleHUD.SetActive(false);
            }

            // run
            if (Input.Q)
            {
                Run();
            }
        }
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
        if (GetComponent<Player>().Elf)
        {
            GetComponent<Player>().Elf.Opponent = Opponent;
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
}

