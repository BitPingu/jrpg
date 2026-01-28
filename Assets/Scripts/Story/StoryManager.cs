using UnityEngine;

public class StoryManager : MonoBehaviour
{
    private static StoryManager Instance;

    [SerializeField] private StoryBase[] _stories;
    public static StoryBase CurrentStory { get; set; }
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
        CurrentStory = _stories[0];
        CurrentStory.BeginStory();
    }

    private void Update()
    {
        CurrentStory.Active();
    }

}
