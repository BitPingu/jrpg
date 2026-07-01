using UnityEngine;

public class React : MonoBehaviour
{
    [SerializeField] private AudioClip _sound;

    private void Start()
    {
        SFXManager.Play(_sound);
    }
}
