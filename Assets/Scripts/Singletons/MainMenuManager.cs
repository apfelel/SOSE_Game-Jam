using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject taskMenu, menu;
    [SerializeField] private CanvasGroup menuGroup, tasksGroup;

    private void Start()
    {
        UIManager.OnUnPause += UnPause;
        Time.timeScale = 1;
    }

    private void OnDisable()
    {
        UIManager.OnUnPause -= UnPause;
    }
    private void UnPause()
    {
        menu.GetComponentsInChildren<Selectable>()[1].Select();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowTasks()
    {
        StartCoroutine(ToTasks());
    }

    public void CloseTasks()
    {
        StartCoroutine(ToMenu());
    }

    public void StartGame(string scene)
    {
        GameManager.Instance.StartGame(scene);
    }
    IEnumerator ToTasks()
    {
        for(int i = 0; i < 30; i++)
        {
            yield return new WaitForFixedUpdate();
            menuGroup.alpha -= 1f / 30;
        }
        tasksGroup.alpha = 0;
        menu.SetActive(false);

        for (int i = 0; i < 40; i++)
        {
            yield return new WaitForFixedUpdate();
            Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, Quaternion.Euler(0, 0, 0), i / 60f);
        }
        Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);

        taskMenu.SetActive(true);
        taskMenu.GetComponentInChildren<Selectable>().Select();
        for (int i = 0; i < 30; i++)
        {
            yield return new WaitForFixedUpdate();
            tasksGroup.alpha += 1f / 30;
        }
    }

    IEnumerator ToMenu()
    {
        for (int i = 0; i < 30; i++)
        {
            yield return new WaitForFixedUpdate();
            tasksGroup.alpha -= 1f / 30;
        }
        tasksGroup.alpha = 0;
        taskMenu.SetActive(false);

        for (int i = 0; i < 40; i++)
        {
            yield return new WaitForFixedUpdate();
            Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, Quaternion.Euler(70, 0, 0), i / 60f);
        }
        Camera.main.transform.rotation = Quaternion.Euler(70, 0, 0);
        menu.SetActive(true);
        menu.GetComponentInChildren<Selectable>().Select();
        for (int i = 0; i < 30; i++)
        {
            yield return new WaitForFixedUpdate();
            menuGroup.alpha += 1f / 30;
        }
    }

}
