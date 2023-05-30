using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject vCam;
    [SerializeField] private GameObject targetVerticalRotation;
    [SerializeField] private float speed;
    [SerializeField] private GameObject visualRoot;

    [SerializeField] private CinemachineFreeLook freelookCam;

    PlayerInput input;
    InputAction moveAction;
    InputAction verticalMoveAction;
    InputAction interactAction;
    InputAction pauseAction;
    InputAction lookAction;
    private Rigidbody rb;
    private Transform cam;
    private PlayerPickup playerPickup;

    private Vector3 targetSpeed;

    public bool Cam;
    

    void Awake()
    {
        vCam.SetActive(false);
        GameManager.LVLFinished += LVLFinished;
        GameManager.LVLStart += UnlockInput;
        UIManager.OnUnPause += UnlockInput;

        cam = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        playerPickup = GetComponentInChildren<PlayerPickup>();

        input = new();
        moveAction = input.Player.Move;

        interactAction = input.Player.Interact;
        interactAction.performed += Interact;

        pauseAction = input.Player.Pause;
        pauseAction.performed += PauseGame;

        lookAction = input.Player.Look;

        verticalMoveAction = input.Player.VerticalMove;
    }

    private void LVLFinished()
    {
        GameManager.LVLFinished -= LVLFinished;
        GameManager.LVLStart -= UnlockInput;
        UIManager.OnUnPause -= UnlockInput;
        this.enabled = false;
    }

    private void PauseGame(InputAction.CallbackContext obj)
    {
        UIManager.Instance.SwitchPause();
        if (UIManager.Instance.IsPaused)
        {
            LockInput();
        }
        else
        {
            UnlockInput();
        }

    }

    private void UnlockInput()
    {
        vCam.SetActive(true);
        pauseAction.Enable();
        verticalMoveAction.Enable();
        moveAction.Enable();
        interactAction.Enable();
        lookAction.Enable();
    }
    private void OnDisable()
    {
        GameManager.LVLFinished -= LVLFinished;
        GameManager.LVLStart -= UnlockInput;
        UIManager.OnUnPause -= UnlockInput;
        pauseAction.Disable();
        verticalMoveAction.Disable();
        LockInput();
    }
    private void LockInput()
    {
        vCam.SetActive(false);
        moveAction.Disable();
        interactAction.Disable();
        lookAction.Disable();
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        playerPickup.Interact();
    }

    void Update()
    {
        if (UIManager.Instance.IsPaused) return;
        Move(moveAction.ReadValue<Vector2>());
        AllignVisualRoot();
    }

    private void Move(Vector2 moveDir)
    {
        var vertical = verticalMoveAction.ReadValue<float>();

        Vector3 newTargetSpeed;
        if (Cam)
        {
            newTargetSpeed = (cam.transform.forward * moveDir.y + cam.transform.right * moveDir.x) * speed;
            newTargetSpeed.y = (vertical * 10);
        }
        else
        {
            newTargetSpeed = (cam.transform.forward * moveDir.y + cam.transform.right * moveDir.x) * speed;
        }


        targetSpeed = newTargetSpeed;

        rb.velocity = Vector3.Lerp(rb.velocity, targetSpeed, Time.deltaTime * 10);

        freelookCam.m_Lens.FieldOfView = Mathf.Lerp(freelookCam.m_Lens.FieldOfView, 40 + rb.velocity.magnitude, Time.deltaTime);
    }
    private void AllignVisualRoot()
    {
        Vector3 targetForward = (rb.velocity + new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z)).normalized;

        float dot = Vector3.Dot(targetForward, visualRoot.transform.forward);
        if (dot < 0.5f)
        {
            targetForward = (Quaternion.Euler(0, 45 * (dot - 0.5f), 0) * targetForward).normalized;
        }

        visualRoot.transform.forward = Vector3.Slerp(
                visualRoot.transform.forward,
                targetForward,
                Time.deltaTime * 10);

        //Quaternion horizontal = Quaternion.LookRotation(targetForward);

        //horizontal.x = 0;
        //horizontal.z = 0;
        //targetVerticalRotation.transform.rotation = horizontal;
        //visualRoot.transform.localRotation = Quaternion.Euler(targetForward.x, 0, 0);

    }


}
