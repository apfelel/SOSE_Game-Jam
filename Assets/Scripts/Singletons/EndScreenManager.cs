using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenManager : MonoSingleton<EndScreenManager>
{
    [SerializeField] private GameObject winScreen, loseScreen;
    private void Start()
    {
        winScreen.SetActive(false);
    }
    public void ShowEndscreen()
    {
        winScreen.SetActive(true);
    }
    public void ShowLoseScreen()
    {
        loseScreen.SetActive(true);
    }
}
