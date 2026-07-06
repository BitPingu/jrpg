using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstQuest : EventBase
{
    public Player PlayerChar { get; set; }
    public Companion Fiona { get; set; }
    public Villager Chief { get; set; }
    public Villager Mom { get; set; }
    public Enemy SlimeChar { get; set; }
    [SerializeField] private Dialogue _fionaDialogue, _momDialogue, _slimeDialogue, _fionaDialogue2, _outBoundsDialogue, _slimeDialogue2, _fionaDialogue3;
    private bool _encounter, _outBounds, _firstSlimeDefeat, _slimeDialogue2Active;

    private void Start()
    {
        // set dialogue delegates
        DialogueController.Instance.OnDialogueFinish += SlimeEncounter;
        DialogueController.Instance.OnDialogueFinish += OutOfBounds;
        DialogueController.Instance.OnBattleDialogueFinish += SlimeDefeat;
        DialogueController.Instance.OnDialogueFinish += SlimeDefeat2;
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
        // first enemy slime encounter
        if (SlimeChar)
        {
            float slimeDistance = Vector2.Distance(SlimeChar.transform.position, PlayerChar.transform.position);
            if (!_encounter && slimeDistance < 3f && !DialogueController.Instance.IsDialogueActive)
            {
                PlayerChar.StateMachine.End(); // stop movement
                Fiona.StateMachine.End(); // stop movement
                
                Fiona.Anim.Rebind();
                Fiona.Anim.enabled = false;

                // start dialogue
                DialogueController.Instance.StartDialogue(_slimeDialogue, new List<CharacterBase>{Fiona});

                CameraController.Instance.target = SlimeChar.transform;

                _encounter = true;
            }
            if (_encounter && slimeDistance > 5f && PlayerChar.StateMachine.CurrentState == PlayerChar.IdleState)
            {
                PlayerChar.StateMachine.End(); // stop movement

                Fiona.Face(PlayerChar);
                Fiona.Anim.Rebind();
                Fiona.Anim.enabled = false;

                // start dialogue
                DialogueController.Instance.StartDialogue(_outBoundsDialogue, new List<CharacterBase>{Fiona});
                _outBounds = true;
            }
        }
    }

    private void SlimeEncounter()
    {
        if (!_encounter || _outBounds || SlimeChar == null)
            return;

        CameraController.Instance.target = PlayerChar.transform;

        PlayerChar.StateMachine.Initialize(PlayerChar.IdleState);
        Fiona.StateMachine.Initialize(Fiona.IdleState);

        Fiona.Anim.enabled = true;
        Fiona.CurrentDialogue = _fionaDialogue2;
    }

    private void OutOfBounds()
    {
        if (!_outBounds)
            return;

        _outBounds = false;

        Fiona.Anim.enabled = true;
        StartCoroutine(GoBack(PlayerChar));
    }

    private IEnumerator GoBack(Player player)
    {
        player.Face(SlimeChar);

        Vector3 returnPos = SlimeChar.transform.position;

        // go back
        float _distance = Vector2.Distance(returnPos, player.transform.position);
        while (_distance > 2f)
        {
            _distance = Vector2.Distance(returnPos, player.transform.position);
            player.Move(returnPos - player.transform.position);
            yield return new WaitForFixedUpdate();
        }

        player.StateMachine.Initialize(player.IdleState); // enable movement
    }

    private void SlimeDefeat()
    {
        if ((SlimeChar && SlimeChar.Opponent == null) || _firstSlimeDefeat)
            return;

        _firstSlimeDefeat = true;

        PlayerChar.StateMachine.End(); // stop movement
        PlayerChar.Face(Fiona);

        Fiona.StateMachine.End();
        Fiona.SpeakAfterBattle = false;

        StartCoroutine(MoveFiona());
    }

    private IEnumerator MoveFiona()
    {
        // move to player
        Fiona.Face(PlayerChar);
        float _distance = Vector2.Distance(PlayerChar.transform.position, Fiona.transform.position);
        while (_distance > 0.7f)
        {
            _distance = Vector2.Distance(PlayerChar.transform.position, Fiona.transform.position);
            Fiona.Move(PlayerChar.transform.position - Fiona.transform.position);
            yield return new WaitForFixedUpdate();
        }

        Fiona.Move(Vector2.zero);

        StartCoroutine(DelayAnimStop());

        // start dialogue
        DialogueController.Instance.StartDialogue(_slimeDialogue2, new List<CharacterBase>{Fiona});
        _slimeDialogue2Active = true;
    }

    private IEnumerator DelayAnimStop()
    {
        yield return new WaitForSeconds(.4f);
        Fiona.Anim.enabled = false;
    }

    private void SlimeDefeat2()
    {
        if (!_slimeDialogue2Active)
            return;
        
        _slimeDialogue2Active = false;

        Fiona.SpeakAfterBattle = true;
        Fiona.CurrentDialogue = _fionaDialogue3;
    }

    private void FinishEvent()
    {
        EventIsDone = true; // event done

        DialogueController.Instance.OnDialogueFinish -= FinishEvent;
    }
}
