using UnityEngine;

public class Marker : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            // StoryManager.Instance.GetComponent<Prologue>().OutOfBounds();
        }
    }
}
