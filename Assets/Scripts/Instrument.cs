using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Instrument : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioSource audioSource;
    public GameObject particleSystemGB;
    public bool Interact()
    {
        var p = Instantiate(particleSystemGB);
        p.transform.parent = null;
        p.transform.position = GetComponentInChildren<VisualEffect>().transform.position;

        audioSource.enabled = false;
        GameManager.Instance.PickupCount++;
        UIManager.Instance.ChangeInteract(false);
        Destroy(this);
        SoundManager.Instance.PlaySound("CleanseLevitation", 1f);
        GetComponentInChildren<VisualEffect>().SetInt("Particle", 0);
        return false;
    }

}
