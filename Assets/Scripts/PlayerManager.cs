using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private GameObject visualRoot;

    PlayerInput input;
    InputAction moveAction;
    InputAction interactAction;

    private Rigidbody rb;
    private Transform cam;
    private PlayerPickup playerPickup;

    private Vector3 targetSpeed;

    

    void Start()
    {
        cam = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        playerPickup = GetComponentInChildren<PlayerPickup>();

        input = new();
        moveAction = input.Player.Move;
        moveAction.Enable();
        interactAction = input.Player.Interact;
        interactAction.performed += Interact;
        interactAction.Enable();
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        playerPickup.Interact();
    }

    void Update()
    {
        Move(moveAction.ReadValue<Vector2>());
        AllignVisualRoot();
    }

    private void Move(Vector2 moveDir)
    {
        Vector3 newTargetSpeed = (cam.transform.forward * moveDir.y + cam.transform.right * moveDir.x) * speed;
        targetSpeed = newTargetSpeed;

        rb.velocity = Vector3.Lerp(rb.velocity, targetSpeed, Time.deltaTime * 5);
    }
    private void AllignVisualRoot()
    {
        visualRoot.transform.forward = Vector3.Slerp(
                visualRoot.transform.forward,
                (rb.velocity + (new Vector3(cam.transform.forward.x, 0,cam.transform.forward.z) * 0.1f)).normalized,
                Time.deltaTime * 5);
    }

}
