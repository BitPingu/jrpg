using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstQuest : EventBase
{
    public Player PlayerChar { get; set; }
    public Companion Fiona { get; set; }
    public Villager Mom { get; set; }
    [SerializeField] private Dialogue _momDialogue;

    private void Start()
    {
        // set dialogue delegates
        // DialogueController.Instance.OnDialogueFinish += FinishEvent;

        // set current dialogue
        Mom.CurrentDialogue = _momDialogue;

        // reset mom position
        Mom.transform.position = new Vector3(.73f,-43.48f,0);
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
