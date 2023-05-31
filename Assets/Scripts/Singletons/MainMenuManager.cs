using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject taskMenu;
    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowTasks()
    {
        taskMenu.SetActive(true);
    }

    public void CloseTasks()
    {
        taskMenu.SetActive(false);
    }

    public void StartGame(string scene)
    {
        GameManager.Instance.StartGame(scene);
    }
}
