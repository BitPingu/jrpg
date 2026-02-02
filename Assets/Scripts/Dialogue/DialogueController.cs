using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; } // Singleton instance

    private Dialogue _dialogue { get; set; }

    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TMP_Text _dialogueText, _nameText;
    [SerializeField] private Image _portraitImage, _continueImage;
    [SerializeField] private Sprite _defaultPortrait;
    [SerializeField] private AudioClip _defaultVoice;

    [SerializeField] private Transform _choiceContainer;
    [SerializeField] private GameObject _choiceButton;

    [SerializeField] private PlayerController _input;

    [SerializeField] private float _autoProgressDelay = 1.5f;
    [SerializeField] private float _typingSpeed = 0.05f;
    [SerializeField] private float _skipDelay = 0.1f;

    private DialogueLine _currentDialogue;
    private int _dialogueIndex = 0;

    private bool _isTyping;
    public bool IsDialogueActive { get; set; }
    private bool _canSkip;
    private bool _prompt;
    private bool _autoProgress;
    public bool IsDialogueFinished { get; set; }

    private Dictionary<string, CharacterBase> _charsInDialogue = new Dictionary<string, CharacterBase>();
    private CharacterBase _currentChar;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // Make sure only one instance
    }

    private void Update()
    {
        if (_input.E && IsDialogueActive && _canSkip && !_prompt && !_autoProgress)
        {
            // next line
            NextLine();
        }
    }

    public void StartDialogue(Dialogue dialogue, List<CharacterBase> characters, bool autoProgress)
    {
        // set params
        _dialogue = dialogue;
        foreach (CharacterBase character in characters)
        {
            _charsInDialogue.Add(character.charName, character);
        }
        _autoProgress = autoProgress;

        IsDialogueActive = true;
        _dialogueIndex = 0;

        // show dialogue
        ShowDialogueUI(true);
        
        DisplayCurrentLine();
    }

    private void ShowDialogueUI(bool show)
    {
        _dialoguePanel.SetActive(show); // Toggle UI visability
    }

    private void NextLine()
    {
        // on key press

        if (_isTyping)
        {
            // skip typing animation and show the full line
            StopAllCoroutines(); // halt auto progress
            SetDialogueText(_currentDialogue.line);
            LineFinish();
        }
        else if (_dialogueIndex+1 < _dialogue.Lines.Length)
        {
            if (_currentDialogue.redirectDialogueIndex > -1)
            {
                // redirect next line
                _dialogueIndex = _currentDialogue.redirectDialogueIndex;
            }
            else
            {
                // next line in dialogue list
                _dialogueIndex++;
            }
            // if another line, type next line
            DisplayCurrentLine();
        }
        else
        {
            // end dialogue
            EndDialogue();
        }
    }

    private IEnumerator DelaySkip()
    {
        _canSkip = false;
        yield return new WaitForSeconds(_skipDelay);
        _canSkip = true;
    }

    private void DisplayCurrentLine()
    {
        StopAllCoroutines();
        UpdateDialogue(); // update dialogue
        StartCoroutine(DelaySkip());
        StartCoroutine(TypeLine());
    }

    private void UpdateDialogue()
    {
        _currentDialogue = _dialogue.Lines[_dialogueIndex];

        if (_charsInDialogue.ContainsKey(_currentDialogue.charName))
        {
            _currentChar = _charsInDialogue[_currentDialogue.charName];
            SetCharInfo(_currentChar.charName, _currentChar.portrait);
        }
        else
        {
            // character name not in dict
            _currentChar = null;
            SetCharInfo(_currentDialogue.charName, _defaultPortrait);
        }
    }

    private void SetCharInfo(string charName, Sprite portrait)
    {
        _nameText.text = charName;
        _portraitImage.sprite = portrait;
    }

    private void SetDialogueText(string text)
    {
        _dialogueText.text = text;
    }

    private IEnumerator TypeLine()
    {
        _isTyping = true;
        _continueImage.enabled = false;

        SetDialogueText("");
        foreach(char letter in _currentDialogue.line)
        {
            SetDialogueText(_dialogueText.text += letter);
            if (_currentChar)
                SFXManager.PlayVoice(_currentChar.voiceSound, _currentChar.voicePitch);
            else
                SFXManager.PlayVoice(_defaultVoice, .6f); // def voice
            yield return new WaitForSeconds(_typingSpeed);
        }

        LineFinish();

        // auto progress
        if (_autoProgress && _dialogue.Lines.Length > _dialogueIndex)
        {
            yield return new WaitForSeconds(_autoProgressDelay);
            NextLine();
        }
    }

    private void LineFinish()
    {
        StopVoice();

        if (_currentDialogue.choices.Length > 0)
        {
            _prompt = true;

            // check if choices and display
            foreach (DialogueChoice choice in _currentDialogue.choices)
            {
                // display
                DisplayChoices(choice);
            }
        }
        else if (!_autoProgress)
        {
            _continueImage.enabled = true;
        }

        _isTyping = false;
    }

    private async void StopVoice()
    {
        await SFXManager.StopVoice();
    }

    private void DisplayChoices(DialogueChoice choice)
    {
        int nextIndex = choice.nextDialogueIndex;
        CreateChoiceButton(choice.choiceText, () => ChooseOption(nextIndex));
    }

    private void CreateChoiceButton(string choiceText, UnityEngine.Events.UnityAction onClick)
    {
        GameObject choiceButton = Instantiate(_choiceButton, _choiceContainer);
        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText;
        choiceButton.GetComponent<Button>().onClick.AddListener(onClick);
        // choiceButton.GetComponent<Button>().onClick.Invoke();
    }

    private void ChooseOption(int nextIndex)
    {
        _dialogueIndex = nextIndex;
        ClearChoices();
        _prompt = false;
        DisplayCurrentLine();
    }

    private void ClearChoices()
    {
        foreach (Transform child in _choiceContainer) Destroy(child.gameObject);
    }

    private void EndDialogue()
    {
        StopAllCoroutines();

        // reset ui
        SetDialogueText("");
        ShowDialogueUI(false);
        _charsInDialogue.Clear();

        // reset conditions
        IsDialogueActive = false;
        _autoProgress = false;
        IsDialogueFinished = true;
    }
}
