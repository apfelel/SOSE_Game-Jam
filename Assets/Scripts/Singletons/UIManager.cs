using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    public static event Action OnUnPause;

    [SerializeField]
    private GameObject HUD;
    [SerializeField]
    private TextMeshProUGUI curPickupsCount;
    [SerializeField]
    private TextMeshProUGUI maxPickups;
    [SerializeField]
    private Image interactionImage;
    [Space]
    [SerializeField]
    private Slider _musicSlider;
    [SerializeField]
    private Slider _sfxSlider;
    [SerializeField]
    private Slider _masterSlider;
    [SerializeField]
    private Slider _ambientSlider;

    [Space]
    [SerializeField]
    private GameObject _pauseMenue;
    [SerializeField]
    private GameObject _settingMenue;

    [Space]
    [HideInInspector]
    public bool IsPaused;
    [HideInInspector]
    public bool InSetting;

    private void Start()
    {
        if (GameManager.Instance.InGame)
        {
            curPickupsCount.text = "0";
            maxPickups.text = GameObject.FindGameObjectsWithTag("Collectable").Count().ToString();

            _pauseMenue.SetActive(false);
            _settingMenue.SetActive(false);
            interactionImage.enabled = false;
        }
    }
    public void Pause()
    {
        IsPaused = true;
        _settingMenue.SetActive(false);
        _pauseMenue.SetActive(true);
        _pauseMenue.GetComponentInChildren<Selectable>()?.Select();
        Time.timeScale = 0.0f;
        if (!GameManager.Instance.InGame) return;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void UnPause()
    {
        InSetting = false;
        IsPaused = false;
        _pauseMenue.SetActive(false);
        _settingMenue.SetActive(false);
        Time.timeScale = 1f;
        if (!GameManager.Instance.InGame) return;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        OnUnPause?.Invoke();
    }

    public void SwitchPause()
    {
        if (IsPaused)
        {
            UnPause();
        }
        else
        {
            Pause();
        }
    }

    public void OpenSettings()
    {
        if(_pauseMenue != null)
            _pauseMenue.SetActive(false);
        _settingMenue.SetActive(true);
        _settingMenue.GetComponentInChildren<Selectable>().Select();

        _musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        _sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume");
        _masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        //_ambientSlider.value = PlayerPrefs.GetFloat("AmbientVolume");

        InSetting = true;
    }

    public void CloseSettings()
    {        
        _settingMenue.SetActive(false);
        if(_pauseMenue != null)
            _pauseMenue.SetActive(true);
        _pauseMenue.GetComponentInChildren<Selectable>().Select();
        PlayerPrefs.Save();
        InSetting = false;
    }
    public void ChangeMusicVolume(float value)
    {
        SoundManager.Instance.ChangeVolume(SoundManager.AudioNames.Music, value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }
    public void ChangeSfxVolume(float value)
    {
        SoundManager.Instance.ChangeVolume(SoundManager.AudioNames.SFX, value);
        PlayerPrefs.SetFloat("SfxVolume", value);
    }
    public void ChangeMasterVolume(float value)
    {
        SoundManager.Instance.ChangeVolume(SoundManager.AudioNames.Master, value);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }
    public void ChangeAmbientVolume(float value)
    {
        SoundManager.Instance.ChangeVolume(SoundManager.AudioNames.Ambient, value);
        PlayerPrefs.SetFloat("AmbientVolume", value);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void BackToMenu()
    {
        UnPause();
        SceneManager.LoadScene("MainMenu");
    }
    public void ChangeInteract(bool active)
    {
        interactionImage.enabled = active;
    }
    public void RefreshPickup()
    {
        curPickupsCount.text = GameManager.Instance.PickupCount.ToString();
    }
}
