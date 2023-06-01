using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public float Timer;

    public static event Action LVLFinished;
    public static event Action LVLStart;


    private bool firstTime = true;
    private int pickupCount = 0;
    private int maxPickups = 0;
    public int PickupCount
    {
        get
        {
            return pickupCount;
        }
        set
        {
            pickupCount = value;
            UIManager.Instance.RefreshPickup();
            if(pickupCount == maxPickups)
                FinishLVL();
        }
    }

    public bool InGame;

    private void Update()
    {
        if (InGame)
            Timer += Time.deltaTime;
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += OnSceneChanged;
        RefreshState();
    }

    private void OnSceneChanged(Scene arg0, Scene arg1)
    {
        RefreshState();
    }

    private void RefreshState()
    {
        if (InGame)
        {
            pickupCount = 0;
            maxPickups = GameObject.FindGameObjectsWithTag("Collectable").Length;
            Timer = 0;
            Time.timeScale = 0;
            UnlockCursor();
        }
        else
        {
            UnlockCursor();
        }
    }
    public void StartGame(string sceneName)
    {
        InGame = true;
        SceneManager.LoadScene(sceneName);
    }
    public void StartLVL()
    {
        if (firstTime)
        {
            UIManager.Instance.ShowTutorial();
            firstTime = false;
        }
        else
        {
            LockCursor();
            LVLStart?.Invoke();
            Time.timeScale = 1;
        }
    }
    public void ReturnToMenu()
    {
        InGame = false;
        SceneManager.LoadScene("Mainmenu");
    }
    private void FinishLVL()
    {
        UnlockCursor();
        Time.timeScale = 0;
        LVLFinished?.Invoke();
        EndScreenManager.Instance.ShowEndscreen();
        InGame = false;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
