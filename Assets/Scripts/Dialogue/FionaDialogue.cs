using UnityEngine;

public class FionaDialogue : Talk
{
    [SerializeField] private Talk _fionaDialogue2;
    [SerializeField] private GameObject _marker;

    // public override void EndDialogue()
    // {
    //     GetComponentInParent<Companion>().Join(_player);

    //     // call base class
    //     base.EndDialogue();

    //     Instantiate(_fionaDialogue2, transform.parent);

    //     // marker
    //     Instantiate(_marker, new Vector3(-1.99f,10f,0f), Quaternion.identity);
        
    //     Destroy(gameObject);
    // }
}
