using UnityEngine;

public class Enemy : CharacterBase
{
    public float SightRadius;
    public float AttackRange;
    public LayerMask playerLayer;
    private Vector2 _curDir;
    private float _moveCounter, _waitCounter, _waitTime;


    protected override void Start()
    {
        // call base class
        base.Start();

        // states
        IdleState = new EnemyIdleState(this, StateMachine);
        BattleState = new EnemyBattleState(this, StateMachine);

        // start in idle state
        StateMachine.Initialize(IdleState);
    }

    protected override void Update()
    {
        // call base class
        base.Update();
    }

    public void SetRandomDir()
    {
        // choose random direction
        _curDir.x = Random.Range(-1f, 1f);
        _curDir.y = Random.Range(-1f, 1f);

        // choose random wait time
        _waitTime = Random.Range(3f, 5f);
    }

    public void InterruptIdle()
    {
        // stop
        _curDir = Vector2.zero;
    }

    public override void Idle()
    {
        // call base class
        base.Idle();

        if (_moveCounter > 0)
        {
            // update move counter
            _moveCounter -= Time.deltaTime;

            // stop
            if (_moveCounter < 0.1f)
                _waitCounter = _waitTime;
        }
        else
        {
            // stop
            InterruptIdle();

            // update wait counter
            _waitCounter -= Time.deltaTime;

            if (_waitCounter < 0)
            {
                SetRandomDir();

                // reset move time
                _moveCounter = 1f;
            }
        }

        // allow enemy movement
        Move(_curDir);
    }

    public void Chase(CharacterBase opponent)
    {
        // chase opponent
        float distance = Vector2.Distance(opponent.transform.position, transform.position);
        if (distance > AttackRange)
        {
            Move(opponent.transform.position - transform.position);
        }
        else
        {
            Move(Vector2.zero);
        }
    }

    public override void Battle()
    {
        // call base class
        base.Battle();

        // TODO: temp
        Move(Vector2.zero);

        // attack opponent


        // opponent escapes
 
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, SightRadius);
    }
}

