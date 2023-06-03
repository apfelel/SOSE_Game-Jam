using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicOnStart : MonoBehaviour
{
    public string musicName;
    private void Start()
    {
        Debug.Log("yese");
        SoundManager.Instance.PlayMusic(musicName);
    }
}
