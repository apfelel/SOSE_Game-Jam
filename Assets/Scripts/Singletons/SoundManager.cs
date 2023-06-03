using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoSingleton<SoundManager>
{

    [SerializeField]
    private AudioMixer _audioMixer;
    [System.Serializable]
    private struct Sound
    {
        public string Name;
        public List<AudioClip> Clips;
    }
    [System.Serializable]
    private struct Music
    {
        public string Name;
        public AudioClip Clip;
    }
    [SerializeField]
    private List<Sound> _soundsList = new();
    [SerializeField]
    private List<Music> _musicList = new();
    [SerializeField]
    private List<Music> _ambientList = new();
    private Dictionary<string, List<AudioClip>> _soundsDic = new();
    private Dictionary<string, AudioClip> _musicDic = new();
    private Dictionary<string, AudioClip> _ambientDic = new();
    private AudioSource _sfxSource;
    private AudioSource _musicSource;
    private AudioSource _ambientSource;

    private AudioReverbZone _reverb;
    private bool _cave;
    public enum AudioNames
    {
        SFX, Music, Master, Ambient
    }
    private void Start()
    {
        _musicSource = transform.GetChild(0).GetComponent<AudioSource>();
        _ambientSource = transform.GetChild(1).GetComponent<AudioSource>();
        _sfxSource = GetComponent<AudioSource>();
        _reverb = GetComponent<AudioReverbZone>();
        var _sfxVolume = PlayerPrefs.GetFloat("SfxVolume", -1);
        if (_sfxVolume == -1)
        {
            _sfxVolume = 0.5f;
            PlayerPrefs.SetFloat("SfxVolume", 0.5f);
        }
        ChangeVolume(AudioNames.SFX, _sfxVolume);

        var _masterVolume = PlayerPrefs.GetFloat("MasterVolume", -1);
        if (_masterVolume == -1)
        {
            _masterVolume = 0.5f;
            PlayerPrefs.SetFloat("MasterVolume", 0.5f);
        }
        ChangeVolume(AudioNames.Master, _masterVolume);

        var _ambientVolume = PlayerPrefs.GetFloat("AmbientVolume", -1);
        if (_ambientVolume == -1)
        {
            _ambientVolume = 0.5f;
            PlayerPrefs.SetFloat("AmbientVolume", 0.5f);
        }
        ChangeVolume(AudioNames.Ambient, _ambientVolume);

        var _musicVolume = PlayerPrefs.GetFloat("MusicVolume", -1);
        if (_musicVolume == -1)
        {
            _musicVolume = 0.5f;
            PlayerPrefs.SetFloat("MusicVolume", 0.5f);
        }
        ChangeVolume(AudioNames.Music, _musicVolume);
       
        
        PlayerPrefs.Save();
        DontDestroyOnLoad(this);
        _soundsList.ForEach((s) => _soundsDic.Add(s.Name, s.Clips));
        _musicList.ForEach((s) => _musicDic.Add(s.Name, s.Clip));
        _ambientList.ForEach((s) => _ambientDic.Add(s.Name, s.Clip));

    }
    private void Update()
    {
        
    }
    public void PlaySound(string name, float volume)
    {
        var clips = _soundsDic[name];
        _sfxSource.PlayOneShot(clips?[Random.Range(0, clips.Count - 1)], volume);
    }

    public void PlayMusic(string name)
    {
        Debug.Log(name);
        _musicSource.clip = _musicDic.GetValueOrDefault(name);
        _musicSource.Play();
    }
    public void ChangeVolume(AudioNames name, float value)
    {
        _audioMixer.SetFloat(name.ToString(), Mathf.Log(value) * 20f);
    }
    public void PlayAmbient(string name)
    {
        _ambientSource.clip = _ambientDic.GetValueOrDefault(name);
        _ambientSource.Play();
    }

    public void SetReverb(bool On)
    {
        _reverb.enabled = On;
    }

    public void ChangeInCave(bool inCave)
    {
        _cave = inCave;
    }
}
