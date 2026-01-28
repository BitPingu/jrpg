using UnityEngine;

[CreateAssetMenu(fileName ="New Dialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public DialogueLine[] Lines;
}

[System.Serializable]
public class DialogueLine
{
    public string charName;
    public string line;
    public bool[] autoProgressLines;

    public DialogueChoice[] choices;
}

[System.Serializable]
public class DialogueChoice
{
    public int dialogueIndex; // Dialogue line where choices appear
    public string[] choices; // Player response options
    public int[] nextDialogueIndexes; // Where choice leads
}
