using UnityEngine;
using UnityEngine.UI;


public class AbilityStateMachine
{
    public Slider Icon;

    public Ability Ability;
    public float CurrentCooldownTime;
    public float CurrentActiveTime;

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
                if (Ability.Condition(_character.gameObject) && !_character.IsAttacking)
                {
                    Debug.Log(_character.name + " used " + Ability.name + ".");
                    _character.IsAttacking = true;
                    Ability.Activate(_character.gameObject);
                    State = AbilityState.active;
                    CurrentActiveTime = Ability.ActiveTime;
                }
            break;
            case AbilityState.active:
                if (CurrentActiveTime > 0)
                {
                    CurrentActiveTime -= Time.deltaTime;
                }
                else
                {
                    _character.IsAttacking = false;
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

