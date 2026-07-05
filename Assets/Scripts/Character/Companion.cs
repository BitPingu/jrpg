using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : PartyBase
{
    public Player Leader { get; set; }

    [SerializeField] private float _followDistance = 1f;
    [SerializeField] private Dialogue _afterBattleDialogue;
    public bool SpeakAfterBattle { get; set; }

    protected override void Start()
    {
        // call base class
        base.Start();

        // set dialogue delegates
        DialogueController.Instance.OnBattleDialogueFinish += AfterBattle;
    }

    public override void Idle()
    {
        // call base class
        base.Idle();

        if (!Leader.IsAttacking)
        {
            if (Leader)
            {
                Follow();
            }
            else
            {
                Move(Vector2.zero);
            }
        }
    }

    public void Join(Player player)
    {
        player.CurrentCompanion = this;
        Leader = player;
        Debug.Log(name + " joins the party!");
    }

    private void Follow()
    {
        float distance = Vector2.Distance(Leader.transform.position, transform.position);
        if (distance > _followDistance)
            Move(Leader.transform.position - transform.position);
        // Stop moving when within range
        if (distance < _followDistance-.2f)
            Move(Vector2.zero);
    }

    public IEnumerator Engage(Vector3 playerAttackPos, float distanceFromEnemy, Vector3 enemyPos)
    {
        // get angle of player attack position in radians
        Vector2 offsetFromCenter = playerAttackPos - enemyPos;
        float currentAngle = Mathf.Atan2(offsetFromCenter.y, offsetFromCenter.x);

        // add offset in radians
        float angleOffsetDegrees = 45f;
        float newAngle = currentAngle + (angleOffsetDegrees * Mathf.Deg2Rad);

        // new position on circumference
        float targetX = enemyPos.x + (distanceFromEnemy * Mathf.Cos(newAngle));
        float targetY = enemyPos.y + (distanceFromEnemy * Mathf.Sin(newAngle));
        Vector3 attackPos = new Vector3(targetX, targetY);
        
        // move to point
        float distance = Vector2.Distance(attackPos, transform.position);
        while (distance > 0.1f && Leader.Opponent != null)
        {
            distance = Vector2.Distance(attackPos, transform.position);
            Move(attackPos - transform.position);
            yield return new WaitForFixedUpdate();
        }

        Opponent = Leader.Opponent;
    }

    private void AfterBattle()
    {
        if (SpeakAfterBattle)
            StartCoroutine(AfterBattleDialogue());
    }

    private IEnumerator AfterBattleDialogue()
    {
        yield return new WaitForSeconds(1f);
        SecondaryDialogueController.Instance.StartDialogue(_afterBattleDialogue, new List<CharacterBase>{this});
    }
}

