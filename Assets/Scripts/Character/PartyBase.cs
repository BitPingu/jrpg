using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
public class PartyBase : FighterBase
{
    public PlayerController Input { get; set; }

    // stats
    [SerializeField] private int _maxExp = 100;
    private int _currentExp = 0;

    // UI
    public Bar EBar;
    public GameObject BattleHUD;

    private bool _battlePrompt;

    protected override void Start()
    {
        // call base class
        base.Start();

        // set components
        Input = GetComponent<PlayerController>();
    }

    public override void Battle()
    {
        // call base class
        base.Battle();

        // attack
        if (BattleTurn)
        {
            // show HUD
            BattleHUD.SetActive(true);

            // battle dialogue
            if (!_battlePrompt)
            {
                string text = "What will " + charName + " do?";
                DialogueController.Instance.BattleDialogue(this, text, true);
                _battlePrompt = true;
            }

            // attack
            if (Input.E)
            {
                StartCoroutine(CallAttack());
                BattleTurn = false;
                _battlePrompt = false;
                BattleHUD.SetActive(false);
            }

            // run (if wild)
            if (Opponent.GetComponent<Enemy>())
            {
                if (Input.Q)
                    StartCoroutine(Run());
            }
            else
            {
                BattleHUD.transform.Find("RunImage").gameObject.SetActive(false);
            }
        }
    }

    public override void Damage(int damageAmount)
    {
        // call base class
        base.Damage(damageAmount);

        if (CurrentHealth <= 0)
        {
            // TODO: temp exit battle state
            // Opponent.Opponent = null;
            // Opponent = null;
            // StartCoroutine(Die());
        }
    }

    public override IEnumerator Die()
    {
        yield return new WaitForSeconds(2f);

        string text = charName + " was defeated!";
        DialogueController.Instance.BattleDialogue(this, text, false);
    }

    public void GainExperience(int amount)
    {
        _currentExp += amount;

        string text = charName + " gained " + amount + " exp.";
        DialogueController.Instance.BattleDialogue(this, text, false);

        StartCoroutine(ShowEBar());

        if (_currentExp >= _maxExp)
        {
            StartCoroutine(LevelUp());
        }
        else
        {
            Debug.Log("Until next level: " + (_maxExp-_currentExp) + " exp");
        }
    }

    private IEnumerator ShowEBar()
    {
        EBar.gameObject.GetComponent<Image>().enabled = true;

        // update health bar
        EBar.UpdateBar(_maxExp, _currentExp);
        yield return new WaitForSeconds(2f);

        EBar.gameObject.GetComponent<Image>().enabled = false;
    }

    private IEnumerator LevelUp()
    {
        // increase level
        Level++;

        // update stats
        MaxHealth = Level * 40;
        CurrentHealth = MaxHealth;
        HBar.UpdateBar(MaxHealth, CurrentHealth);
        Strength = Level * 3 + 3;

        // reset exp
        _currentExp = 0;
        _maxExp = Level * 100;

        yield return new WaitForSeconds(.3f);

        // glow up effect
        Debug.Log(name + " grew to level " + Level + "!");
        Sprite.material.SetColor("_FlashColor", new Color(0, 243, 255));
        CallDamageFlash();

        yield return new WaitForSeconds(1f);

        Sprite.material.SetColor("_FlashColor", Color.white);

        EBar.UpdateBar(_maxExp, _currentExp);
    }

    protected virtual IEnumerator Run()
    {
        // end battle
        // delay
        yield return new WaitForSeconds(1f);

        // TODO: add random chance of working
        if (Opponent)
            Opponent.Opponent = null;
        Opponent = null;
    }
}

