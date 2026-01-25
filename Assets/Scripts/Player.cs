using UnityEngine;

public class Player : CharacterBase
{
    public PlayerController Input { get; set; }

    public Enemy NearbyEnemy { get; set; }

    [field: SerializeField] private GameObject _engageIcon;
    private GameObject _iconRef;


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

        // allow player movement all states
        Move(new Vector2(Input.HorizontalInput, Input.VerticalInput));

        // engage enemy
        if (NearbyEnemy != null && Input.E)
        {
            Destroy(_iconRef);
            NearbyEnemy.InterruptIdle();
            Opponent = NearbyEnemy;
            // NearbyEnemy = null;
            StartCoroutine(Attack());
        }
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

