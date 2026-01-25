using UnityEngine;

public class Companion : CharacterBase
{
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
            float distance = Vector2.Distance(_player.transform.position, transform.position);
            if (distance > _minDistance)
                Move(_player.transform.position - transform.position);
            // Stop moving when within range
            if (distance < _minDistance-0.3)
                Move(Vector2.zero);
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
}
