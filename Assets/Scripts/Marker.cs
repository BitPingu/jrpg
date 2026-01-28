using UnityEngine;

public class Marker : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            StoryManager.CurrentStory.GetComponent<Prologue>().OutOfBounds();
        }
    }
}
