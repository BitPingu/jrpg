using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; } // Singleton instance

    public Dialogue dialogue { get; set; }

    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TMP_Text _dialogueText, _nameText;
    [SerializeField] private Image _portraitImage, _continueImage;
    [SerializeField] private Sprite _defPortrait;

    [SerializeField] private Transform _choiceContainer;
    [SerializeField] private GameObject _choiceButton;

    [SerializeField] private PlayerController _input;

    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;

    private DialogueLine _currentDialogue;
    private int _dialogueIndex = 0;

    private bool _isTyping;
    public bool IsDialogueActive { get; set; }
    public bool DelaySkip { get; set; }
    public bool Prompt { get; set; }
    public bool IsDialogueFinished { get; set; }

    public Dictionary<string, CharacterBase> CharsInDialogue = new Dictionary<string, CharacterBase>();
    private CharacterBase _currentChar;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // Make sure only one instance
    }

    private void Update()
    {
        if (_input.E && IsDialogueActive && !DelaySkip && !Prompt)
        {
            // next line
            NextLine();
        }
    }

    public void StartDialogue()
    {
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
        else if (_dialogueIndex+1 < dialogue.Lines.Length)
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

    private void DisplayCurrentLine()
    {
        StopAllCoroutines();
        UpdateDialogue(); // update dialogue
        StartCoroutine(TypeLine());
    }

    private void UpdateDialogue()
    {
        _currentDialogue = dialogue.Lines[_dialogueIndex];

        if (CharsInDialogue.ContainsKey(_currentDialogue.charName))
        {
            _currentChar = CharsInDialogue[_currentDialogue.charName];
            SetCharInfo(_currentChar.charName, _currentChar.portrait);
        }
        else
        {
            // not in dict
            SetCharInfo(_currentDialogue.charName, _defPortrait);
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
            SFXManager.PlayVoice(_currentChar.voiceSound, _currentChar.voicePitch);
            yield return new WaitForSeconds(typingSpeed);
        }

        LineFinish();

        // auto progress
        // if (_currentDialogue.autoProgressLines.Length > _dialogueIndex && _currentDialogue.autoProgressLines[_dialogueIndex])
        // {
        //     yield return new WaitForSeconds(autoProgressDelay);
        //     NextLine();
        // }
    }

    private void LineFinish()
    {
        StopVoice();

        if (_currentDialogue.choices.Length > 0)
        {
            Prompt = true;

            // check if choices and display
            foreach (DialogueChoice choice in _currentDialogue.choices)
            {
                // display
                DisplayChoices(choice);
            }
        }
        else
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
        Prompt = false;
        DisplayCurrentLine();
    }

    private void ClearChoices()
    {
        foreach (Transform child in _choiceContainer) Destroy(child.gameObject);
    }

    private void EndDialogue()
    {
        StopAllCoroutines();
        IsDialogueActive = false;
        SetDialogueText("");
        ShowDialogueUI(false);
        IsDialogueFinished = true;
        CharsInDialogue.Clear();
    }
}
