using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerRefresh : MonoBehaviour
{
    public int index;
    private void Start()
    {
        GetComponent<TextMeshProUGUI>().text = GameManager.Instance.taskTimes[index] == 0? "/": GameManager.Instance.taskTimes[index].ToString("n2");
    }
}
