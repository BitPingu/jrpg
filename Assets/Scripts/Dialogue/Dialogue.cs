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
    // public bool[] autoProgressLines;

    public DialogueChoice[] choices;
    public int redirectDialogueIndex = -1;
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText; // Player response option
    public int nextDialogueIndex; // Where choice leads
}
