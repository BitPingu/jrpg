using UnityEngine;
using System.Collections;

public class Slime : Enemy
{
    public AbilityStateMachine Lunge;

    protected override void Awake()
    {
        // call base class
        base.Awake();

        // ability
        foreach (Ability ability in Abilities)
        {
            if (ability.name == "Lunge")
            {
                Lunge = new AbilityStateMachine(ability, GetComponent<Enemy>(), KeyCode.None);
                Lunge.State = AbilityStateMachine.AbilityState.cooldown;
                Lunge.CurrentCooldownTime = 1f;
            }
        }
    }

    public override void UseAbilities()
    {
        if (Lunge != null)
            Lunge.Update();
    }

    public void UseLunge()
    {
        StartCoroutine(LungeRaycast());
    }

    IEnumerator LungeRaycast()
    {
        Vector2 _targetVector = Opponent.transform.position - transform.position;

        float _elapsedTime = 0f;
        while (_elapsedTime < Lunge.Ability.ActiveTime)
        {
            // iterate timer
            _elapsedTime += Time.fixedDeltaTime;

            // apply force
            RB.linearVelocity = _targetVector * MoveSpeed * 1.5f;

            yield return new WaitForFixedUpdate();
        }

        Anim.SetTrigger("Attack");
    }
}

