using UnityEngine;

[System.Serializable]
public class VillagerDialogue
{
    public Villager villager;
    public Dialogue dialogue;
}

public class Prologue : ChapterBase
{
    // characters
    [SerializeField] protected Player _player;
    [SerializeField] protected Companion _fiona;
    [SerializeField] protected Villager _mom, _chief, _shopkeeper;
    [SerializeField] protected VillagerDialogue[] _vDialogue;
    [SerializeField] protected GameObject _chiefHouse;

    // dialogue
    // [SerializeField] protected Dialogue _chiefDialogue1;
    // [SerializeField] protected Dialogue _shopDialogue;

    public override void BeginChapter()
    {
        // villagers
        foreach (VillagerDialogue vd in _vDialogue)
        {
            vd.villager.CurrentDialogue = vd.dialogue;
        }

        base.BeginChapter();
    }

    public override void SetupEvent()
    {
        base.SetupEvent();

        if (CurrentEvent.GetComponent<TalkToMom>())
        {
            _player.CanEnter = true; // enable enter action
            CurrentEvent.GetComponent<TalkToMom>().PlayerChar = _player;
            CurrentEvent.GetComponent<TalkToMom>().Mom = _mom;
        }
        else if (CurrentEvent.GetComponent<TalkToFiona>())
        {
            CurrentEvent.GetComponent<TalkToFiona>().PlayerChar = _player;
            CurrentEvent.GetComponent<TalkToFiona>().Fiona = _fiona;
        }
        else if (CurrentEvent.GetComponent<HeadToMatch>())
        {
            _player.CanEnter = false; // disable enter action
            CurrentEvent.GetComponent<HeadToMatch>().PlayerChar = _player;
            CurrentEvent.GetComponent<HeadToMatch>().Fiona = _fiona;
            CurrentEvent.GetComponent<HeadToMatch>().Chief = _chief;
        }
        else if (CurrentEvent.GetComponent<SparringMatch>())
        {
            CurrentEvent.GetComponent<SparringMatch>().PlayerChar = _player;
            CurrentEvent.GetComponent<SparringMatch>().Fiona = _fiona;
            CurrentEvent.GetComponent<SparringMatch>().Chief = _chief;
            CurrentEvent.GetComponent<SparringMatch>().House = _chiefHouse;
        }
        else if (CurrentEvent.GetComponent<FirstQuest>())
        {
            CurrentEvent.GetComponent<FirstQuest>().PlayerChar = _player;
            CurrentEvent.GetComponent<FirstQuest>().Fiona = _fiona;
        }
    }
}
