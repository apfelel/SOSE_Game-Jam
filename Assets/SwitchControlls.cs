using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchControlls : MonoBehaviour
{

    public GameObject keyboard, gamepad;
    void OnEnable()
    {
        if(GameManager.Instance.Keyboard)
        {
            keyboard.SetActive(true);
            gamepad.SetActive(false);
        }
        else
        {
            keyboard.SetActive(false);
            gamepad.SetActive(true);
        }
    }

}
