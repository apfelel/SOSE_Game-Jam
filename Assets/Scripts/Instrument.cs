using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrument : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioSource audioSource;
    public void Interact()
    {
        audioSource.enabled = false;
        GameManager.Instance.PickupCount++;
        UIManager.Instance.ChangeInteract(false);
    }

}
