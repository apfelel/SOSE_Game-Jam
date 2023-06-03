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
        
        StartCoroutine(WaitSec());
        UIManager.Instance.ChangeInteract(false);
        return false;
    }

    IEnumerator WaitSec()
    {
       

        Destroy(GetComponent<Collider>());
        yield return new WaitForSeconds(1.2f);
        var p = Instantiate(particleSystemGB);
        p.transform.parent = null;
        p.transform.position = GetComponentInChildren<VisualEffect>().transform.position;

        GameManager.Instance.PickupCount++;
        audioSource.enabled = false;
        GetComponentInChildren<VisualEffect>().SetInt("Particle", 0);
        SoundManager.Instance.PlaySound("CleanseLevitation", 1f);
        Destroy(this);
    }

}
