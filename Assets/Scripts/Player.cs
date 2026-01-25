using UnityEngine;

public class Player : CharacterBase
{
    public PlayerController Input { get; set; }

    private Enemy _nearbyEnemy { get; set; }
    private Chest _nearbyChest { get; set; }

    [field: SerializeField] private GameObject _battleIcon, _interactIcon;
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
        // enter battle
        if (StateMachine.CurrentState == IdleState && hitInfo.GetComponent<Enemy>())
        {
            _iconRef = Instantiate(_battleIcon, hitInfo.transform);
            _nearbyEnemy = hitInfo.GetComponent<Enemy>();
        }

        // enter interact
        if (StateMachine.CurrentState == IdleState && hitInfo.GetComponent<Chest>())
        {
            _iconRef = Instantiate(_interactIcon, hitInfo.transform);
            _nearbyChest = hitInfo.GetComponent<Chest>();
        }
    }

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        // exit battle
        if (StateMachine.CurrentState == IdleState && hitInfo.GetComponent<Enemy>())
        {
            _nearbyEnemy = null;
            Destroy(_iconRef);
        }

        // exit interact
        if (StateMachine.CurrentState == IdleState && hitInfo.GetComponent<Chest>())
        {
            _nearbyChest = null;
            Destroy(_iconRef);
        }
    }

    public override void Idle()
    {
        // call base class
        base.Idle();

        // allow player movement
        Move(new Vector2(Input.HorizontalInput, Input.VerticalInput));

        // engage enemy
        if (_nearbyEnemy != null && Input.E)
        {
            Destroy(_iconRef);
            _nearbyEnemy.InterruptIdle();
            Opponent = _nearbyEnemy;
            StartCoroutine(Attack());
        }

        // open chest
        if (_nearbyChest != null && Input.E)
        {
            Destroy(_iconRef);
            _nearbyChest.Open();
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

