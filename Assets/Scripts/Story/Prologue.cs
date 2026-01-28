using UnityEngine;

public class Prologue : StoryBase
{
    [SerializeField] private CharacterBase _Fiona;
    [SerializeField] private Talk _fionaDialogue;

    public override void BeginStory()
    {
        base.BeginStory();

        CharacterBase fiona = Instantiate(_Fiona, new Vector3(-1.48f,-0.18f,0), Quaternion.identity);
        Instantiate(_fionaDialogue, fiona.transform);
    }
}
