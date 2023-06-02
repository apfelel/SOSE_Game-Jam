using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [HideInInspector] public bool InRange;
    private IInteractable collectable;
    private PlayerAnimController animController;
    public GameObject lastPickupGb;
    public Vector3 lastPickupPos;
    private Collider col;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        animController = GetComponentInParent<PlayerAnimController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(lastPickupGb)
            lastPickupPos = lastPickupGb.transform.position;
    }

    public void Interact()
    {
        if (!InRange) return;
        col.enabled = false;
        col.enabled = true;
        collectable.Interact();
        animController.PlayCleaning();
        collectable = null;
        lastPickupGb = null;
        InRange = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable col))
        {
            collectable = col;
            InRange = true;
            lastPickupGb = other.gameObject;
            UIManager.Instance.ChangeInteract(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable _))
        {
            collectable = null;
            lastPickupGb = null;
            InRange = false;
            UIManager.Instance.ChangeInteract(false);
        }
    }
}
