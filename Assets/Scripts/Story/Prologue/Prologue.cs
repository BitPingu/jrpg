using UnityEngine;

public class Prologue : ChapterBase
{
    // characters
    [SerializeField] protected Player _player;
    [SerializeField] protected Companion _fiona;
    [SerializeField] protected Villager _mom, _villager1, _villager2, _villager3, _villager4, _chief;

    // dialogue
    [SerializeField] protected Dialogue _fionaDialogue1, _fionaDialogue2;
    [SerializeField] protected Dialogue _momDialogue1, _momDialogue2;
    [SerializeField] protected Dialogue _chiefDialogue1;
    [SerializeField] protected Dialogue _vDialogue1, _vDialogue2, _vDialogue3, _vDialogue4;

    public override void BeginChapter()
    {
        base.BeginChapter();

        // player
        _player.transform.position = new Vector3(-13.79f,-41.41f);
        _player.CanEnter = true;

        // villagers
        _villager1.CurrentDialogue = _vDialogue1;
        _villager2.CurrentDialogue = _vDialogue2;
        _villager3.CurrentDialogue = _vDialogue3;
        _villager4.CurrentDialogue = _vDialogue4;

        // start first event
        CurrentEvent = Instantiate(Events[EventIndex], transform);
        SetupEvent();
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
            _mom.CurrentDialogue = _momDialogue1;
            CurrentEvent.GetComponent<TalkToFiona>().Fiona = _fiona;
        }
        else if (CurrentEvent.GetComponent<HeadToFestival>())
        {
            _player.CanEnter = false; // disable enter action
            CurrentEvent.GetComponent<HeadToFestival>().PlayerChar = _player;
            _fiona.CurrentDialogue = _fionaDialogue1;
            CurrentEvent.GetComponent<HeadToFestival>().Fiona = _fiona;
        }
        else if (CurrentEvent.GetComponent<Festival>())
        {
            CurrentEvent.GetComponent<Festival>().PlayerChar = _player;
            CurrentEvent.GetComponent<Festival>().Fiona = _fiona;
            _mom.CurrentDialogue = _momDialogue2;
            CurrentEvent.GetComponent<Festival>().Mom = _mom;
            _chief.CurrentDialogue = _chiefDialogue1;
            CurrentEvent.GetComponent<Festival>().Chief = _chief;
        }
        else if (CurrentEvent.GetComponent<FightFiona>())
        {
            CurrentEvent.GetComponent<FightFiona>().PlayerChar = _player;
            CurrentEvent.GetComponent<FightFiona>().Fiona = _fiona;
        }
        else if (CurrentEvent.GetComponent<FetchQuest>())
        {
            _fiona.CurrentDialogue = _fionaDialogue2;
        }
    }
}
