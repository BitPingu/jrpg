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

    // dialogue
    // [SerializeField] protected Dialogue _chiefDialogue1;
    // [SerializeField] protected Dialogue _shopDialogue;

    public override void BeginChapter()
    {
        // player
        _player.transform.position = new Vector3(-13.79f,-41.41f);
        _player.CanEnter = true;

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
            CurrentEvent.GetComponent<TalkToMom>().Mom = _mom;
        }
        else if (CurrentEvent.GetComponent<TalkToFiona>())
        {
            CurrentEvent.GetComponent<TalkToFiona>().Fiona = _fiona;
        }
        // else if (CurrentEvent.GetComponent<HeadToMatch>())
        // {
        //     _player.CanEnter = false; // disable enter action
        //     CurrentEvent.GetComponent<HeadToMatch>().PlayerChar = _player;
        //     _fiona.CurrentDialogue = _fionaDialogue1;
        //     CurrentEvent.GetComponent<HeadToMatch>().Fiona = _fiona;
        // }
        // else if (CurrentEvent.GetComponent<SparringMatch>())
        // {
        //     CurrentEvent.GetComponent<SparringMatch>().PlayerChar = _player;
        //     CurrentEvent.GetComponent<SparringMatch>().Fiona = _fiona;
        //     CurrentEvent.GetComponent<SparringMatch>().Chief = _chief;
        // }
        // else if (CurrentEvent.GetComponent<FetchQuest>())
        // {
        //     _fiona.CurrentDialogue = _fionaDialogue2;
        // }
    }
}
