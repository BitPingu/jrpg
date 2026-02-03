using UnityEngine;

public class PrologueTest : Prologue
{
    public override void BeginChapter()
    {
        base.BeginChapter();

        Debug.Log("Begin test");

        // player
        _player.transform.position = new Vector3(0f,0f);
        _fiona.Join(_player);
    }
}
