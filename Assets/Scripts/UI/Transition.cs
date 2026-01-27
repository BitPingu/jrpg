using System.Threading.Tasks;
using UnityEngine;

public class Transition : MonoBehaviour
{
    public static Transition Instance;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private CameraController _camera;
    private float _smooth;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _smooth = _camera.smoothTime;
    }

    async Task Fade(float targetTransparency)
    {
        float start = _canvasGroup.alpha, t=0;
        while (t < _fadeDuration)
        {
            t += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(start, targetTransparency, t / _fadeDuration);
            await Task.Yield();
        }
        _canvasGroup.alpha = targetTransparency;
    }

    public async Task FadeOut()
    {
        await Fade(1); // Fade to black
        _camera.smoothTime = 0f;
    }

    public async Task FadeIn()
    {
        await Fade(0); // Fade to transparent
        _camera.smoothTime = _smooth;
    }
}
