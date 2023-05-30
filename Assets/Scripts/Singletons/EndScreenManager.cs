using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenManager : MonoSingleton<EndScreenManager>
{
    [SerializeField] private GameObject screen;
    private void Start()
    {
        screen.SetActive(false);
    }
    public void ShowEndscreen()
    {
        screen.SetActive(true);
    }
}
