using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable
{
    public bool destroy;
    public GameObject particleSystemGB;

    public bool Interact()
    {
       

        UIManager.Instance.ChangeInteract(false);

        if (destroy)
        {
            StartCoroutine(WaitSec());
            return true;
        }
        else
        {
            StartCoroutine(WaitSec2());
            Destroy(this);
            return false;
        }
    }
    IEnumerator WaitSec()
    {
       
        Destroy(GetComponent<Collider>());
        yield return new WaitForSeconds(1.3f);
        var p = Instantiate(particleSystemGB);
        p.transform.parent = null;
        p.transform.position = transform.position;

        GameManager.Instance.PickupCount++; 
        SoundManager.Instance.PlaySound("CleanseLevitation", 1f);
        Destroy(gameObject);

    }
    IEnumerator WaitSec2()
    {
        Destroy(GetComponent<Collider>());
        yield return new WaitForSeconds(1.3f);
        var p = Instantiate(particleSystemGB);
        p.transform.parent = null;
        p.transform.position = transform.position;

        GameManager.Instance.PickupCount++;
        Destroy(this);
        SoundManager.Instance.PlaySound("CleanseLevitation", 1f);
    }
}
