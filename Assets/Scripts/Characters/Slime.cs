using UnityEngine;
using System.Collections;

public class Slime : Enemy
{
    // public AbilityStateMachine Lunge;

    protected override void Awake()
    {
        // call base class
        base.Awake();

        // ability
        // foreach (Ability ability in Abilities)
        // {
        //     if (ability.name == "Lunge")
        //     {
        //         Lunge = new AbilityStateMachine(ability, GetComponent<Enemy>(), KeyCode.None);
        //         Lunge.State = AbilityStateMachine.AbilityState.cooldown;
        //         Lunge.CurrentCooldownTime = 1f;
        //     }
        // }
    }

    // public void UseLunge()
    // {
    //     StartCoroutine(LungeRaycast());
    // }

    // IEnumerator LungeRaycast()
    // {
    //     Lunge.Ability.IsActive = true;

    //     yield return new WaitForSeconds(1f);
    //     Anim.SetTrigger("Attack");
    //     yield return new WaitForSeconds(1f);

    //     Lunge.Ability.IsActive = false;
    // }
}

