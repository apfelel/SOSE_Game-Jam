using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [HideInInspector] public bool InRange;
    private GameObject collectible;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        if (!InRange) return;

        Destroy(collectible);
        GameManager.Instance.PickupCount++;
        UIManager.Instance.ChangeInteract(false);
        InRange = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectable"))
        {
            collectible = other.gameObject;
            InRange = true;
            UIManager.Instance.ChangeInteract(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Collectable"))
        {
            collectible = null;
            InRange = false;
            UIManager.Instance.ChangeInteract(false);
        }
    }
}
