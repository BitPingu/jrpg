using System.Collections;
using UnityEngine;

public class Companion : CharacterBase
{
    public PlayerController Input;

    private Player _player { get; set; }

    [SerializeField] private float _minDistance = 1.55f;

    protected override void Start()
    {
        // call base class
        base.Start();

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

        if (_player)
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
        _player = player;

        Debug.Log(name + " joins the party!");
    }

    private void Follow()
    {
        float distance = Vector2.Distance(_player.transform.position, transform.position);
        if (distance > _minDistance)
            Move(_player.transform.position - transform.position);
        // Stop moving when within range
        if (distance < _minDistance-0.3)
            Move(Vector2.zero);
    }

    public void CallBattlePos(Vector3 playerAttackPos)
    {
        StartCoroutine(BattlePos(playerAttackPos));
    }

    private IEnumerator BattlePos(Vector3 playerAttackPos)
    {
        Vector3 attackDir = (Opponent.transform.position - playerAttackPos).normalized;
        Vector3 attackPos = (Opponent.transform.position - (attackDir*2.5f)) + _player.transform.up;

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
        // call base class
        base.Battle();

        // attack
        if (BattleTurn && Input.E)
        {
            StartCoroutine(CallAttack());
            BattleTurn = false;
        }
    }
}
