using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance { get; private set; } // Singleton instance

    [SerializeField] private StoryBase[] _stories;
    public StoryBase CurrentStory { get; set; }
    private int _storyIndex;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // Make sure only one instance
    }

    private void Start()
    {
        CurrentStory = _stories[0];
        CurrentStory.BeginStory();
    }

    private void Update()
    {
        CurrentStory.Active();
    }

}
