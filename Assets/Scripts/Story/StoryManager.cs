using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance { get; private set; } // Singleton instance

    [SerializeField] private ChapterBase[] _chapters;
    public ChapterBase CurrentChapter { get; set; }
    private int _storyIndex;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // Make sure only one instance
    }

    private void Start()
    {
        CurrentChapter = _chapters[0];
        CurrentChapter.BeginChapter();
    }

    private void Update()
    {
        CurrentChapter.Active();
    }

}
