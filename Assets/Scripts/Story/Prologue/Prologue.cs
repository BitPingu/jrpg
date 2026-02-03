using UnityEngine;

public class Prologue : ChapterBase
{
    // characters
    [SerializeField] private Player _player;
    [SerializeField] private Companion _fiona;
    [SerializeField] private Villager _mom, _villager1, _villager2, _villager3, _villager4, _chief;

    // dialogue
    [SerializeField] private Dialogue _vDialogue1, _vDialogue2, _vDialogue3, _vDialogue4;

    public override void BeginStory()
    {
        base.BeginStory();

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
        CurrentEvent.GetComponent<TalkToMom>().Mom = _mom;
    }

    public override void NextEvent()
    {
        base.NextEvent();

        if (CurrentEvent.GetComponent<TalkToFiona>())
        {
            CurrentEvent.GetComponent<TalkToFiona>().Fiona = _fiona;
        }
        if (CurrentEvent.GetComponent<HeadToFestival>())
        {
            CurrentEvent.GetComponent<HeadToFestival>().PlayerChar = _player;
            CurrentEvent.GetComponent<HeadToFestival>().Fiona = _fiona;
        }
        else if (CurrentEvent.GetComponent<Festival>())
        {
            CurrentEvent.GetComponent<Festival>().PlayerChar = _player;
            CurrentEvent.GetComponent<Festival>().Fiona = _fiona;
            CurrentEvent.GetComponent<Festival>().Mom = _mom;
            CurrentEvent.GetComponent<Festival>().Chief = _chief;
        }
        else if (CurrentEvent.GetComponent<FightFiona>())
        {
            CurrentEvent.GetComponent<FightFiona>().PlayerChar = _player;
            CurrentEvent.GetComponent<FightFiona>().Fiona = _fiona;
        }
    }
}
