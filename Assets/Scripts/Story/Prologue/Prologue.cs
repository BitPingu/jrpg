using UnityEngine;

public class Prologue : StoryBase
{
    [SerializeField] private CharacterBase _Mom, _Fiona, _player, _fiona;

    public override void BeginStory()
    {
        base.BeginStory();

        // player
        _player = GameObject.Find("Player").GetComponent<Player>();
        _player.transform.position = new Vector3(-14.77f,-42.68f,0);

        // mom
        // CharacterBase mom = Instantiate(_Mom, GameObject.Find("Player House").transform.position + new Vector3(-4.94f,-1.69f,0), Quaternion.identity, GameObject.Find("Player House").transform);

        // fiona
        // _fiona = Instantiate(_Fiona, new Vector3(-1.48f,-0.18f,0), Quaternion.identity);
    }

    public override void Active()
    {

    }
}
