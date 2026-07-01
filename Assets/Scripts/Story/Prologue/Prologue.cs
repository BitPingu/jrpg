using UnityEngine;

public class Prologue : ChapterBase
{
    // characters
    [SerializeField] protected Player _player;
    [SerializeField] protected Companion _fiona;
    [SerializeField] protected Villager _mom, _chief, _shopkeeper;
    [SerializeField] protected Villager _villager1, _villager2, _villager3, _villager4;

    // dialogue
    [SerializeField] protected Dialogue _fionaDialogue1, _fionaDialogue2;
    [SerializeField] protected Dialogue _momDialogue1, _momDialogue2;
    [SerializeField] protected Dialogue _chiefDialogue1;
    [SerializeField] protected Dialogue _vDialogue1, _vDialogue2, _vDialogue3, _vDialogue4;
    [SerializeField] protected Dialogue _shopDialogue;

    public override void BeginChapter()
    {
        base.BeginChapter();

        // player
        // _player.transform.position = new Vector3(-13.79f,-41.41f);
        // _player.CanEnter = true;

        // villagers
        // _villager1.CurrentDialogue = _vDialogue1;
        // _villager2.CurrentDialogue = _vDialogue2;
        // _villager3.CurrentDialogue = _vDialogue3;
        // _villager4.CurrentDialogue = _vDialogue4;

        // _shopkeeper.CurrentDialogue = _shopDialogue;
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
        else if (CurrentEvent.GetComponent<HeadToMatch>())
        {
            _player.CanEnter = false; // disable enter action
            CurrentEvent.GetComponent<HeadToMatch>().PlayerChar = _player;
            _fiona.CurrentDialogue = _fionaDialogue1;
            CurrentEvent.GetComponent<HeadToMatch>().Fiona = _fiona;
        }
        else if (CurrentEvent.GetComponent<SparringMatch>())
        {
            CurrentEvent.GetComponent<SparringMatch>().PlayerChar = _player;
            CurrentEvent.GetComponent<SparringMatch>().Fiona = _fiona;
            CurrentEvent.GetComponent<SparringMatch>().Chief = _chief;
        }
        else if (CurrentEvent.GetComponent<FetchQuest>())
        {
            _fiona.CurrentDialogue = _fionaDialogue2;
        }
    }
}
