using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    [SerializeField] private SFXGroup[] _sfxGroups;
    private Dictionary<string, List<AudioClip>> _sFXDictionary;

    private void Awake()
    {
        InitDict();
    }

    private void InitDict()
    {
        _sFXDictionary = new Dictionary<string, List<AudioClip>>();
        foreach (SFXGroup sFXGroup in _sfxGroups)
        {
            _sFXDictionary[sFXGroup.name] = sFXGroup.audioClips;
        }
    }

    public AudioClip GetClip(string name)
    {
        if (_sFXDictionary.ContainsKey(name))
        {
            return _sFXDictionary[name][0];
        }
        return null;
    }
}

[System.Serializable]
public struct SFXGroup
{
    public string name;
    public List<AudioClip> audioClips;
}
