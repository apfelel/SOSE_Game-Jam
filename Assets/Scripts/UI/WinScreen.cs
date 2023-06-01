using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    public int task;

    private void OnEnable()
    {
        GameManager.Instance.task[task] = true;
        GameManager.Instance.taskTimes[task] = GameManager.Instance.Timer;
    }
}
