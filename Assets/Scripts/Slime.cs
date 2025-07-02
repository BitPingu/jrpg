using UnityEngine;
using System.Collections;

public class Slime : Enemy
{
    public AbilityStateMachine Lunge;

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
        yield return new WaitForSeconds(.5f);

        // Animate attack
        // Anim.SetTrigger("Lunge");

        Vector3 _lungeForce = transform.forward * ((LungeAbility)Lunge.Ability).ForceMultiplier;

        // force
        // float _elapsedTime = 0f;
        // while (_elapsedTime < .8f && !IsStun)
        // {
        //     // iterate timer
        //     _elapsedTime += Time.fixedDeltaTime;

        //     // gravity
        //     _lungeForce.y += -9.81f*Time.deltaTime;

        //     // apply force
        //     if (Controller.enabled)
        //         Controller.Move(_lungeForce*Time.deltaTime);

        //     yield return new WaitForFixedUpdate();
        // }

        yield return new WaitForSeconds(1f);

        // Animate attack
        // Anim.SetTrigger("Lunge");

        yield return new WaitForSeconds(1f);
    }
}

