using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SFXManager : MonoBehaviour
{
    private static SFXManager Instance;

    private static AudioSource _audioSource;
    private static AudioSource _voiceAudioSource;
    private static SFX _sFXLibrary;
    [SerializeField] private Slider _sFXSlider;

    private static float _voiceVol;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            AudioSource[] audioSources = GetComponents<AudioSource>();
            _audioSource = audioSources[0];
            _voiceAudioSource = audioSources[1];
            _voiceVol = audioSources[1].volume;
            _sFXLibrary = GetComponent<SFX>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // _sFXSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
    }

    public static void Play(string soundName)
    {
        AudioClip audioClip = _sFXLibrary.GetClip(soundName);
        if (audioClip != null)
        {
            _audioSource.PlayOneShot(audioClip);
        }
    }

    public static void PlayVoice(AudioClip audioClip, float pitch = 1f)
    {
        _voiceAudioSource.volume = _voiceVol;
        _voiceAudioSource.pitch = pitch;
        _voiceAudioSource.PlayOneShot(audioClip);
    }

    public static async Task StopVoice()
    {
        await FadeVoice();
    }

    private static async Task FadeVoice()
    {
        float start = _voiceAudioSource.volume;
        float fadeDuration = .1f;
        while (_voiceAudioSource.volume > 0f)
        {
            _voiceAudioSource.volume -= start * Time.deltaTime / fadeDuration;
            await Task.Yield();
        }
        _voiceAudioSource.Stop();
    }

    public static void SetVolume(float volume)
    {
        _audioSource.volume = volume;
        _voiceAudioSource.volume = volume;
    }

    public void OnValueChanged()
    {
        SetVolume(_sFXSlider.value);
    }
}
