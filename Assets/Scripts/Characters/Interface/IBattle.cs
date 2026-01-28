using System.Collections;
using UnityEngine;

public interface IBattle
{
    public StateBase BattleState { get; set; }

    public CharacterBase Opponent { get; set; }

    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }

    public float Strength { get; set; }

    // public List<Ability> Abilities;

    public Coroutine DamageFlashCoroutine { get; set; }

    public bool BattleTurn { get; set; }
    public bool IsAttacking { get; set; }
    public bool WinBattle { get; set; }

    public Bar HBar { get; set; }
    public Bar EBar { get; set; }
    public GameObject BattleHUD { get; set; }

    public GameObject Projectile { get; set; }

    public AudioClip HitSound { get; set; }

    public void Battle();

    public IEnumerator CallAttack();
    public IEnumerator Attack();
    public IEnumerator RangedAttack();
    public void Damage(float damageAmount);
    public IEnumerator Die();
    public void CallDamageFlash();
    public IEnumerator DamageFlash();

    public void GainExperience(int amount);
    public IEnumerator ShowEBar();
    public IEnumerator LevelUp();
    public void Run();
}
