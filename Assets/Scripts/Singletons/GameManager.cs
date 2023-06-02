using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public bool[] task = new bool[3];
    public float[] taskTimes = new float[3];

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
        if (!InGame) return;

        Timer += Time.deltaTime;
        if(Timer > 60 * 5)
        {
            LoseLVL();
        }
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
        }
        else
        {
            if(task.All(t => t == true))
            {
                Debug.Log("win");
            }
        }
        UnlockCursor();
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

    private void LoseLVL()
    {
        UnlockCursor();
        Time.timeScale = 0;
        LVLFinished?.Invoke();
        EndScreenManager.Instance.ShowLoseScreen();
        InGame = false;
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
