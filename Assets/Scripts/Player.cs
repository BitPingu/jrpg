using UnityEngine;
using System.Collections;

public class Player : CharacterBase
{
    public PlayerController Input { get; set; }

    public CharacterBase NearbyEnemy { get; set; }
    public bool IsEngaging { get; set; }

    [field: SerializeField] private GameObject _engageIcon;
    private GameObject _iconRef;


    protected override void Start()
    {
        // call base class
        base.Start();

        // player components
        Input = GetComponent<PlayerController>();

        // states
        IdleState = new PlayerIdleState(this, StateMachine);
        BattleState = new PlayerBattleState(this, StateMachine);

        // start in idle state
        StateMachine.Initialize(IdleState);
    }

    protected override void Update()
    {
        // call base class
        base.Update();

        // allow player movement all states
        if (!IsEngaging && !IsAttacking)
            Move(new Vector2(Input.HorizontalInput, Input.VerticalInput));
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (StateMachine.CurrentState == IdleState && hitInfo.GetComponent<Enemy>())
        {
            _iconRef = Instantiate(_engageIcon, hitInfo.transform);
            NearbyEnemy = hitInfo.GetComponent<Enemy>();
        }
    }

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (StateMachine.CurrentState == IdleState && hitInfo.GetComponent<Enemy>())
        {
            NearbyEnemy = null;
            Destroy(_iconRef);
        }
    }

    public override void Idle()
    {
        // call base class
        base.Idle();

        // engage enemy
        if (NearbyEnemy != null && Input.E)
        {
            Destroy(_iconRef);
            StartCoroutine(Engage());
        }
    }

    IEnumerator Engage()
    {
        IsEngaging = true;

        float _distance;
        do
        {
            _distance = Vector2.Distance(NearbyEnemy.transform.position, transform.position);
            Move(NearbyEnemy.transform.position - transform.position);
            yield return new WaitForFixedUpdate();
        } while (_distance > .4f);

        Anim.SetTrigger("Engage");

        yield return new WaitForSeconds(.1f);

        Opponent = NearbyEnemy;
        NearbyEnemy = null;
        Opponent.Opponent = this;

        IsEngaging = false;
    }

    public override void Battle()
    {
        // call base class
        base.Battle();

        // attack
        if (Input.E)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        IsAttacking = true;

        Vector2 _targetVector = Opponent.transform.position - transform.position;

        float _elapsedTime = 0f;
        while (_elapsedTime < .12f)
        {
            // iterate timer
            _elapsedTime += Time.fixedDeltaTime;

            // apply force
            RB.linearVelocity = _targetVector * MoveSpeed * 2f;

            yield return new WaitForFixedUpdate();
        }

        Anim.SetTrigger("Attack");

        IsAttacking = false;
    }
}

