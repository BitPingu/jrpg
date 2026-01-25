using UnityEngine;
using UnityEngine.UI;


public class AbilityStateMachine
{
    public Slider Icon;

    public Ability Ability;
    public float CurrentCooldownTime;

    public enum AbilityState
    {
        ready,
        active,
        cooldown
    }
    public AbilityState State;

    private CharacterBase _character;
    private KeyCode _key;


    public AbilityStateMachine(Ability ability, CharacterBase character, KeyCode key)
    {
        // setup ability
        Ability = ability;

        Ability.IsActive = false;
        State = AbilityState.ready;

        // player abilities
        _character = character;
    }

    public void Update()
    {
        switch (State)
        {
            case AbilityState.ready:
                if (Ability.Condition(_character.gameObject))
                {
                    Debug.Log(_character.name + " used " + Ability.name + ".");
                    Ability.Activate(_character.gameObject);
                    State = AbilityState.active;
                }
            break;
            case AbilityState.active:
                if (!Ability.IsActive)
                {
                    Ability.BeginCooldown(_character.gameObject);
                    State = AbilityState.cooldown;
                    CurrentCooldownTime = Ability.CooldownTime;
                }
            break;
            case AbilityState.cooldown:
                if (CurrentCooldownTime > 0)
                {
                    CurrentCooldownTime -= Time.deltaTime;
                }
                else
                {
                    State = AbilityState.ready;
                    // Debug.Log(Ability.name + " is ready.");
                }
            break;
        }
    }
}

