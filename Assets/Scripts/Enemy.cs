using UnityEngine;

public class Enemy : CharacterBase
{
    [field: SerializeField] public float SightRadius { get; set; }
    [field: SerializeField] public float AttackRange { get; set; }
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

    public override void Idle()
    {
        // call base class
        base.Idle();

        if (_moveCounter > 0)
        {
            // Debug.Log("idle move");
            // update move counter
            _moveCounter -= Time.deltaTime;

            // stop
            if (_moveCounter < 0.1f)
                _waitCounter = _waitTime;
        }
        else
        {
            // Debug.Log("idle wait");
            // stop
            _curDir = Vector2.zero;

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

        // chase opponent
        Chase(Opponent);

        // opponent escapes
        float distance = Vector2.Distance(Opponent.transform.position, transform.position);
        if (distance > SightRadius)
        {
            Opponent.Opponent = null;
            Opponent = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, SightRadius);
    }
}

