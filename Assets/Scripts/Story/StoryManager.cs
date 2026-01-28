using UnityEngine;

public class StoryManager : MonoBehaviour
{
    private static StoryManager Instance;

    [SerializeField] private StoryBase[] _stories;
    public StoryBase ActiveStory { get; set; }
    private int _storyIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ActiveStory = _stories[0];
        ActiveStory.BeginStory();
    }


}
