using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public static event Action LVLFinished;
    public static event Action LVLStart;

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

    private void Start()
    {
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
            maxPickups = GameObject.FindGameObjectsWithTag("Collectable").Length;
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
        LockCursor();
        LVLStart?.Invoke();
        Time.timeScale = 1;
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
