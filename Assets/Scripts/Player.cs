using UnityEngine;

public class Player : CharacterBase
{
    public PlayerController Input { get; set; }

    private Enemy _nearbyEnemy { get; set; }
    private Chest _nearbyChest { get; set; }
    private Companion _nearbyCompanion { get; set; }
    public Companion Elf { get; set; }

    [field: SerializeField] private GameObject _battleIcon, _interactIcon;
    private GameObject _battleIconRef, _interactIconRef;


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
            _battleIconRef = Instantiate(_battleIcon, hitInfo.transform);
            _nearbyEnemy = hitInfo.GetComponent<Enemy>();
        }

        // enter interact
        if (StateMachine.CurrentState == IdleState && hitInfo.GetComponent<Chest>())
        {
            _interactIconRef = Instantiate(_interactIcon, hitInfo.transform);
            _nearbyChest = hitInfo.GetComponent<Chest>();
        }
        if (StateMachine.CurrentState == IdleState && Elf == null && hitInfo.GetComponent<Companion>())
        {
            _interactIconRef = Instantiate(_interactIcon, hitInfo.transform);
            _nearbyCompanion = hitInfo.GetComponent<Companion>();
        }
    }

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        // exit battle
        if (StateMachine.CurrentState == IdleState && hitInfo.GetComponent<Enemy>())
        {
            _nearbyEnemy = null;
            Destroy(_battleIconRef);
        }

        // exit interact
        if (StateMachine.CurrentState == IdleState && hitInfo.GetComponent<Chest>())
        {
            _nearbyChest = null;
            Destroy(_interactIconRef);
        }
        if (StateMachine.CurrentState == IdleState && Elf == null && hitInfo.GetComponent<Companion>())
        {
            _nearbyCompanion = null;
            Destroy(_interactIconRef);
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
            Destroy(_battleIconRef);
            _nearbyEnemy.InterruptIdle();
            Opponent = _nearbyEnemy;
            StartCoroutine(Attack());
        }

        // open chest
        if (_nearbyChest != null && Input.E)
        {
            Destroy(_interactIconRef);
            _nearbyChest.Open();
        }

        // talk to companion
        if (_nearbyCompanion != null && Input.E)
        {
            Destroy(_interactIconRef);
            Elf = _nearbyCompanion;
            Elf.Join(this);
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

