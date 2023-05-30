using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{

    private int pickupCount = 0;
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
        }
    }

    public bool InGame;

    public void StartGame(string sceneName)
    {
        InGame = true;
        SceneManager.LoadScene(sceneName);
    }
    public void ReturnToMenu()
    {
        InGame = false;
    }
}
