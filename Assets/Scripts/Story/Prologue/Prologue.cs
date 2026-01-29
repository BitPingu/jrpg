using System.Collections;
using UnityEngine;

public class Prologue : StoryBase
{
    [SerializeField] private Player _player;
    [SerializeField] private Companion _fiona;
    [SerializeField] private Villager _mom, _villager1, _villager2;
    [SerializeField] private Dialogue _momDialogue, _vDialogue1, _vDialogue2;

    public override void BeginStory()
    {
        base.BeginStory();

        // player
        _player.transform.position = new Vector3(-14.77f,-42.68f,0);
        _player.CanEnter = true;

        // mom
        _mom.CurrentDialogue = _momDialogue;

        // villagers
        _villager1.CurrentDialogue = _vDialogue1;
        _villager2.CurrentDialogue = _vDialogue2;

        // start first event
        CurrentEvent = Instantiate(Events[EventIndex], transform);
        CurrentEvent.GetComponent<TalkToFiona>().Fiona = _fiona;
    }

    public override void Active()
    {
        base.Active();

        if (CurrentEvent && CurrentEvent.EventIsDone)
        {
            StartCoroutine(NextEvent());
        }
    }

    public override IEnumerator NextEvent()
    {
        base.NextEvent();

        Destroy(CurrentEvent.gameObject); // end event

        yield return new WaitForSeconds(0f);

        CurrentEvent = Instantiate(Events[++EventIndex], transform); // next event

        if (CurrentEvent.GetComponent<HeadToFestival>())
        {
            CurrentEvent.GetComponent<HeadToFestival>().PlayerChar = _player;
            CurrentEvent.GetComponent<HeadToFestival>().Fiona = _fiona;
        }
    }
}
