using UnityEngine;

public class Player : CharacterBase
{
    public CharacterBase NearbyEnemy { get; set; }
    public bool Engaging { get; set; }

    [field: SerializeField] private GameObject _engageIcon;
    private GameObject _iconRef;


    protected override void Start()
    {
        // call base class
        base.Start();

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
        if (!Engaging)
            Move(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
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

    public void Engage()
    {
        // Get distance from pos
        float distance = Vector2.Distance(NearbyEnemy.transform.position, transform.position);

        if (distance > .3f)
        {
            Move(NearbyEnemy.transform.position - transform.position);
        }
        else
        {
            Engaging = false;
            Anim.SetTrigger("Engage");
            Opponent = NearbyEnemy;
            NearbyEnemy = null;
            Opponent.Opponent = this;
        }
    }

    public override void Idle()
    {
        // call base class
        base.Idle();

        // engage enemy
        if (NearbyEnemy != null && Input.GetKeyDown(KeyCode.E))
        {
            Destroy(_iconRef);
            Engaging = true;
        }

        if (Engaging)
        {
            Engage();
        }
    }

    public override void Battle()
    {
        // call base class
        base.Battle();
    }
}

