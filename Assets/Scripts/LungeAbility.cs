using UnityEngine;

[CreateAssetMenu]
public class LungeAbility : Ability
{
    public int Power = 5;

    public override void Activate(GameObject parent)
    {
        base.Activate(parent);

        Slime slime =  parent.GetComponent<Slime>();

        // call ability from character
        // slime.UseLunge();

        // Debug.Log(name + " activated.");
    }

    public override void BeginCooldown(GameObject parent)
    {
        base.BeginCooldown(parent);

        Slime slime =  parent.GetComponent<Slime>();
    
        // ability finished
        Debug.Log(name + " on cooldown.");
    }
}

