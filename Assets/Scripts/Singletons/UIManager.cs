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
    public static event Action OnPause;
    public static event Action<float> OnSettingsClosed;

    [SerializeField]
    private Image staminaImage;
    [SerializeField] 
    private GameObject tutorial, startScreen;
    [SerializeField]
    private GameObject HUD;
    [SerializeField]
    private TextMeshProUGUI curPickupsCount;
    [SerializeField]
    private TextMeshProUGUI maxPickups;
    [SerializeField]
    private TextMeshProUGUI timer;
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
    [SerializeField]
    private Slider _sensitivitySlider;

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
        var sensitivity = PlayerPrefs.GetFloat("Sensitivity", -1);
        if (sensitivity == -1)
        {
            sensitivity = 1;
            PlayerPrefs.SetFloat("Sensitivity", 1);
        }

        if (GameManager.Instance.InGame)
        {
            HUD.SetActive(true);
            startScreen.SetActive(true);
            tutorial.SetActive(false);
            curPickupsCount.text = "0";
            maxPickups.text = GameObject.FindGameObjectsWithTag("Collectable").Count().ToString();
        }
        else
            HUD.SetActive(false);
        _pauseMenue.SetActive(false);
        _settingMenue.SetActive(false);
        interactionImage.enabled = false;

        OnSettingsClosed?.Invoke(sensitivity);
    }
    private void Update()
    {
        if(GameManager.Instance.InGame && timer)
            timer.text = GameManager.Instance.Timer.ToString("n2");
    }
    public void Pause()
    {
        OnPause?.Invoke();
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
        PlayerPrefs.Save();
        OnSettingsClosed?.Invoke(PlayerPrefs.GetFloat("Sensitivity"));
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
        _ambientSlider.value = PlayerPrefs.GetFloat("AmbientVolume");
        _sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
        InSetting = true;
    }

    public void CloseSettings()
    {
        PlayerPrefs.Save();
        OnSettingsClosed?.Invoke(PlayerPrefs.GetFloat("Sensitivity"));
        InSetting = false;
        if (GameManager.Instance.InGame)
        {
            _settingMenue.SetActive(false);
            _pauseMenue.SetActive(true);
            _pauseMenue.GetComponentInChildren<Selectable>().Select();
            UnPause();
        }
        else
        {
            _settingMenue.SetActive(false);
            _pauseMenue.SetActive(false);
        }
        
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
    public void ChangeSensitivity(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
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

    //public void FadeIn(bool instant = false)
    //{
    //    if(!instant)
    //        StartCoroutine(Fade(-1));
    //    else
    //        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
    //}
    //IEnumerator Fade(float mod)
    //{
    //    for(int i = 0; i < 30; i++)
    //    {
    //        yield return new WaitForEndOfFrame();
    //        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, (1 / 30f) * mod);
    //    }
    //    fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, Mathf.Clamp01(mod));
    //}
    //public void FadeOut(bool instant = false)
    //{
    //    if (!instant)
    //        StartCoroutine(Fade(1));
    //    else
    //        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);
    //}

    public void ShowTutorial()
    {
        tutorial.gameObject.SetActive(true);
    }

    public void StartLVL()
    {
        GameManager.Instance.StartLVL();
    }
    public void ReturnToMenu()
    {
        GameManager.Instance.ReturnToMenu();
    }

    public void RefreshStamina(float ratio)
    {
        staminaImage.fillAmount = ratio;
    }
}
