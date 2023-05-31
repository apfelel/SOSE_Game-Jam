using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Destroy(gameObject);
        GameManager.Instance.PickupCount++;
        UIManager.Instance.ChangeInteract(false);
    }

}
