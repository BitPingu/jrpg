using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : PartyBase
{
    public Companion CurrentCompanion { get; set; }

    // conditions
    public bool CanEnter { get; set; }
    public bool IsEntering { get; set; }
    public bool Entered { get; set; }

    public bool StatusOn { get; set; }
    [SerializeField] private GameObject _inventoryBox;

    protected override void Update()
    {
        // call base class
        base.Update();

        if (Input.Q && StateMachine.CurrentState == IdleState && !DialogueController.Instance.IsDialogueActive)
        {
            Debug.Log("RUN!");
            StateMachine.End(); // disable movement
            StatusOn = true;
            CheckStatus();
        }
        else if (Input.Q && StatusOn)
        {
            StateMachine.Initialize(IdleState); // enable movement
            StatusOn = false;
            CheckStatus();
        }
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
        Opponent.Damage(0);

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

    protected override IEnumerator Run()
    {
        // call base class
        base.Run();

        if (CurrentCompanion)
        {
            CurrentCompanion.Opponent = null;
        }

        yield return null;
    }

    public void CheckStatus()
    {
        if (StatusOn)
        {
            // show bars
            HBar.gameObject.GetComponent<Image>().enabled = true;
            EBar.gameObject.GetComponent<Image>().enabled = true;
            if (CurrentCompanion)
            {
                CurrentCompanion.HBar.gameObject.GetComponent<Image>().enabled = true;
                CurrentCompanion.EBar.gameObject.GetComponent<Image>().enabled = true;
            }

            // inventory
            _inventoryBox.SetActive(true);

            if (Input.E)
            {
                // TODO: need to make this more dynamic
                ItemBase item = InventoryController.Instance.GetItem("Potion");
                if (item is Consumable)
                {
                    Debug.Log(name + " used a " + item.name + ".");
                    Heal(((Consumable)item).amount);
                }
                else
                {
                    Debug.Log("There are no consumable items.");
                }
            }
        }
        else if (!StatusOn && StateMachine.CurrentState == IdleState)
        {
            // hide bars
            HBar.gameObject.GetComponent<Image>().enabled = false;
            EBar.gameObject.GetComponent<Image>().enabled = false;
            if (CurrentCompanion)
            {
                CurrentCompanion.HBar.gameObject.GetComponent<Image>().enabled = false;
                CurrentCompanion.EBar.gameObject.GetComponent<Image>().enabled = false;
            }

            // inventory
            _inventoryBox.SetActive(false);
        }
    }
}

