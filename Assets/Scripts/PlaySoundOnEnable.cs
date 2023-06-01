using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnEnable : MonoBehaviour
{
    [SerializeField] private string sfxName;
    private void OnEnable()
    {
        SoundManager.Instance.PlaySound(sfxName, 1f);
    }
}
