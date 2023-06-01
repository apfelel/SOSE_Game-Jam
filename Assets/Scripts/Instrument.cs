using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrument : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioSource audioSource;
    public GameObject particleSystemGB;
    public bool Interact()
    {
        var p = Instantiate(particleSystemGB);
        p.transform.parent = null;
        p.transform.position = transform.position;

        audioSource.enabled = false;
        GameManager.Instance.PickupCount++;
        UIManager.Instance.ChangeInteract(false);
        enabled = false;
        return false;
    }

}
