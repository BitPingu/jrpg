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
    public Enemy SlimeChar1 { get; set; }
    public Enemy SlimeChar2 { get; set; }
    [SerializeField] private GameObject _reactIcon;
    [SerializeField] private Dialogue _fionaDialogue, _momDialogue, _slimeDialogue, _fionaDialogue2, _outBoundsDialogue, _slimeDialogue2, _afterBattleDialogue, _fionaDialogue3, _ambushDialogue, _afterAmbushDialogue;
    private bool _encounter, _outBounds, _firstSlimeDefeat, _slimeDialogue2Active, _encounter2;
    private int _inPos;

    private void Start()
    {
        // set dialogue delegates
        DialogueController.Instance.OnDialogueFinish += SlimeEncounter;
        DialogueController.Instance.OnDialogueFinish += OutOfBounds;
        DialogueController.Instance.OnBattleDialogueFinish += SlimeDefeat;
        DialogueController.Instance.OnDialogueFinish += SlimeDefeat2;
        DialogueController.Instance.OnDialogueFinish += Ambush;
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

        // ambush
        if (SlimeChar1 && SlimeChar2)
        {
            float slimeDistance = Vector2.Distance(SlimeChar1.transform.position, PlayerChar.transform.position);
            if (!_encounter2 && slimeDistance < 3.3f)
            {
                PlayerChar.StateMachine.End(); // stop movement
                Fiona.StateMachine.End(); // stop movement

                Fiona.Anim.Rebind();
                Fiona.Anim.enabled = false;

                SlimeChar1.Face(PlayerChar);
                SlimeChar2.Face(Fiona);

                SlimeChar1.StateMachine.End();
                SlimeChar2.StateMachine.End();

                StartCoroutine(MoveSlime(SlimeChar1, new Vector2(PlayerChar.transform.position.x-1.6f, PlayerChar.transform.position.y+.5f)));
                StartCoroutine(MoveSlime(SlimeChar2, new Vector2(Fiona.transform.position.x+1.6f, Fiona.transform.position.y-.3f)));

                _encounter2 = true;
            }
            if (_inPos == 2)
            {
                _inPos = -1;

                StartCoroutine(AmbushReact());
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
        if ((SlimeChar && SlimeChar.Opponents.Count == 0) || _firstSlimeDefeat)
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
        Fiona.CurAfterBattleDialogue = _afterBattleDialogue;

        PlayerChar.StateMachine.Initialize(PlayerChar.IdleState);
        Fiona.StateMachine.Initialize(Fiona.IdleState);
    }

    private IEnumerator MoveSlime(CharacterBase character, Vector3 destination)
    {
        // move to battle position
        float distance = Vector2.Distance(destination, character.transform.position);
        Vector2 vec = destination - character.transform.position;
        while (distance > 0.1f)
        {
            distance = Vector2.Distance(destination, character.transform.position);
            character.Move(vec);
            yield return new WaitForFixedUpdate();
        }

        character.Move(Vector2.zero);

        _inPos++;
    }

    private IEnumerator AmbushReact()
    {
        // react
        Vector2 iconPos = new Vector2(Fiona.transform.position.x, Fiona.transform.position.y+1f);
        GameObject _activeIcon = Instantiate(_reactIcon, iconPos, Quaternion.identity, Fiona.transform);
        yield return new WaitForSeconds(1f);
        Destroy(_activeIcon);

        // start dialogue
        DialogueController.Instance.StartDialogue(_ambushDialogue, new List<CharacterBase>{Fiona});
    }

    private void Ambush()
    {
        if (!_encounter2 || (SlimeChar1 == null && SlimeChar2 == null))
            return;
        
        PlayerChar.StateMachine.Initialize(PlayerChar.IdleState);
        Fiona.StateMachine.Initialize(Fiona.IdleState);
        Fiona.Anim.enabled = true;
        Fiona.CurAfterBattleDialogue = _afterAmbushDialogue;

        SlimeChar1.StateMachine.Initialize(SlimeChar1.IdleState);
        SlimeChar2.StateMachine.Initialize(SlimeChar2.IdleState);

        PlayerChar.Opponents.AddRange(new List<FighterBase>{SlimeChar1, SlimeChar2});
        Fiona.Opponents.AddRange(new List<FighterBase>{SlimeChar1, SlimeChar2});

        SlimeChar1.Opponents.AddRange(new List<FighterBase>{PlayerChar, Fiona});
        SlimeChar1.Allies.Add(SlimeChar2);
        SlimeChar2.Opponents.AddRange(new List<FighterBase>{PlayerChar, Fiona});
        SlimeChar2.Allies.Add(SlimeChar1);

        // Slime goes first
        SlimeChar1.BattleTurn = true;
    }

    private void FinishEvent()
    {
        EventIsDone = true; // event done

        DialogueController.Instance.OnDialogueFinish -= FinishEvent;
    }
}
