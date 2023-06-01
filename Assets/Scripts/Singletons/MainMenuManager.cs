using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject taskMenu, menu;
    [SerializeField] private CanvasGroup menuGroup, tasksGroup;
    [SerializeField] private CinemachineVirtualCamera menuCam, taskMenuCam;

    //[SerializeField] private List<CinemachineVirtualCamera> taskCams;
    //[SerializeField] private List<CanvasGroup> taskGroups;
    //[SerializeField] private List<GameObject> taskMenus;
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
    //public void ShopDetailedTask(int task)
    //{
    //    StartCoroutine(ToTask(task));
    //}
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

        menuCam.Priority = 0;
        taskMenuCam.Priority = 10;
        yield return new WaitForSeconds(0.5f);

        taskMenu.SetActive(true);
        taskMenu.GetComponentInChildren<Selectable>().Select();
        for (int i = 0; i < 30; i++)
        {
            yield return new WaitForFixedUpdate();
            tasksGroup.alpha += 1f / 30;
        }
    }

    //IEnumerator ToTask(int task)
    //{
    //    for (int i = 0; i < 30; i++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //        tasksGroup.alpha -= 1f / 30;
    //    }
    //    tasksGroup.alpha = 0;
    //    taskMenu.SetActive(false);

    //    taskMenuCam.Priority = 0;
    //    taskCams[task].Priority = 10;
    //    yield return new WaitForSeconds(0.5f);

    //    taskMenus[task].SetActive(true);
    //    taskMenus[task].GetComponentInChildren<Selectable>().Select();
    //    for (int i = 0; i < 30; i++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //        taskGroups[task].alpha += 1f / 30;
    //    }
    //}
    //IEnumerator ToTaskMenu(int task)
    //{
    //    for (int i = 0; i < 30; i++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //        taskGroups[task].alpha -= 1f / 30;
    //    }
    //    taskGroups[task].alpha = 0;
    //    taskMenus[task].SetActive(false);

    //    taskCams[task].Priority = 0;
    //    taskMenuCam.Priority = 10;
    //    yield return new WaitForSeconds(0.5f);

    //    taskMenu.SetActive(true);
    //    taskMenu.GetComponentInChildren<Selectable>().Select();
    //    for (int i = 0; i < 30; i++)
    //    {
    //        yield return new WaitForFixedUpdate();
    //        tasksGroup.alpha += 1f / 30;
    //    }
    //}
    IEnumerator ToMenu()
    {
        for (int i = 0; i < 30; i++)
        {
            yield return new WaitForFixedUpdate();
            tasksGroup.alpha -= 1f / 30;
        }
        tasksGroup.alpha = 0;
        taskMenu.SetActive(false);

        menuCam.Priority = 10;
        taskMenuCam.Priority = 0;
        yield return new WaitForSeconds(0.5f);

        menu.SetActive(true);
        menu.GetComponentInChildren<Selectable>().Select();
        for (int i = 0; i < 30; i++)
        {
            yield return new WaitForFixedUpdate();
            menuGroup.alpha += 1f / 30;
        }
    }

}
