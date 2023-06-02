using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable
{
    public bool destroy;
    public GameObject particleSystemGB;

    public bool Interact()
    {
        var p = Instantiate(particleSystemGB);
        p.transform.parent = null;
        p.transform.position = transform.position;

        GameManager.Instance.PickupCount++;
        UIManager.Instance.ChangeInteract(false);

        if (destroy)
        {
            Destroy(gameObject);
            return true;
        }
        else
        {
            SoundManager.Instance.PlaySound("CleanseLevitation", 1f);
            GetComponentInParent<LevitatingObject>().ResetPosition();
            Destroy(this);
            return false;
        }
    }

}
