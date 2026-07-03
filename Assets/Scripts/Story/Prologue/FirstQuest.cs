using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstQuest : EventBase
{
    public Player PlayerChar { get; set; }
    public Companion Fiona { get; set; }
    public Villager Chief { get; set; }
    public Villager Mom { get; set; }
    [SerializeField] private Dialogue _fionaDialogue, _momDialogue;

    private void Start()
    {
        // set dialogue delegates
        // DialogueController.Instance.OnDialogueFinish += FinishEvent;

        // set current dialogues
        Fiona.CurrentDialogue = _fionaDialogue;
        Mom.CurrentDialogue = _momDialogue;

        // reset chief position
        Chief.gameObject.SetActive(false); // TODO: temp before moving to house

        // reset mom position
        Mom.transform.position = new Vector3(.73f,-43.48f,0);
        Mom.Sprite.flipX = false;
    }

    private void Update()
    {
        
    }

    private void FinishEvent()
    {
        EventIsDone = true; // event done

        DialogueController.Instance.OnDialogueFinish -= FinishEvent;
    }
}
