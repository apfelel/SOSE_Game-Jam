using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [HideInInspector] public bool InRange;
    private IInteractable collectible;
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
        collectible.Interact();
        collectible = null;
        InRange = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable col))
        {
            collectible = col;
            InRange = true;
            UIManager.Instance.ChangeInteract(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable _))
        {
            collectible = null;
            InRange = false;
            UIManager.Instance.ChangeInteract(false);
        }
    }
}
