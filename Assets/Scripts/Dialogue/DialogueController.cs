using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; } // Singleton instance

    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage, continueImage;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // Make sure only one instance
    }

    public void ShowDialogueUI(bool show)
    {
        dialoguePanel.SetActive(show); // Toggle UI visability
    }

    public void SetNPCInfo(string npcName, Sprite portrait)
    {
        nameText.text = npcName;
        portraitImage.sprite = portrait;
    }

    public void SetDialogueText(string text)
    {
        dialogueText.text = text;
    }
}
