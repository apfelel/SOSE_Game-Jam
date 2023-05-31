using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private LayerMask moveAllignLayer;
    [SerializeField] private GameObject vCam;
    [SerializeField] private GameObject targetVerticalRotation;
    [SerializeField] private float speed;
    [SerializeField] private GameObject visualRoot;

    [SerializeField] private CinemachineFreeLook freelookCam;
    [SerializeField] private float maxHeight;
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

    void FixedUpdate()
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


        targetSpeed = newTargetSpeed * Mathf.Clamp(Vector3.Dot(visualRoot.transform.forward, newTargetSpeed.normalized) + 0.4f, 0.1f, 1f);
        RaycastHit hit;
        var targetMag = targetSpeed.magnitude;
        var rayDistance = targetMag * 0.5f;

        if(Physics.Raycast(transform.position, targetSpeed.normalized, out hit, rayDistance, moveAllignLayer)
            || Physics.Raycast(transform.position + (visualRoot.transform.right * 0.2f), targetSpeed.normalized + (visualRoot.transform.right * 0.2f), out hit, rayDistance, moveAllignLayer)
            || Physics.Raycast(transform.position + (visualRoot.transform.right * -0.2f), targetSpeed.normalized + (visualRoot.transform.right * -0.2f), out hit, rayDistance, moveAllignLayer)
            || Physics.Raycast(transform.position + (visualRoot.transform.up * 0.2f), targetSpeed.normalized + (visualRoot.transform.up * -0.2f), out hit, rayDistance, moveAllignLayer)
            || Physics.Raycast(transform.position + (visualRoot.transform.up * -0.2f), targetSpeed.normalized + (visualRoot.transform.up * -0.2f), out hit, rayDistance, moveAllignLayer))
        {
            var projected = Vector3.ProjectOnPlane(targetSpeed, hit.normal);
            targetSpeed = (targetSpeed.normalized + projected.normalized * 0.5f).normalized * targetMag;
        }

        if(transform.position.y > maxHeight && targetSpeed.y > 0)
        {
            var forward = targetSpeed;
            forward.y = 0;
            targetSpeed = (targetSpeed.normalized + (forward.normalized * (transform.position.y - maxHeight))).normalized * targetMag;
        }

        rb.velocity = Vector3.Lerp(rb.velocity, targetSpeed, Time.deltaTime * 10);

        freelookCam.m_Lens.FieldOfView = Mathf.Lerp(freelookCam.m_Lens.FieldOfView, 40 + rb.velocity.magnitude, Time.deltaTime);
    }
    private void AllignVisualRoot()
    {
        Vector3 targetForward = rb.velocity;

        if (Mathf.Abs(Vector3.Angle(targetForward, visualRoot.transform.forward)) < 5) return;

        float dot = Vector3.Dot(targetForward, visualRoot.transform.forward);
        if (dot < 0.5f)
        {
            targetForward = targetForward + (visualRoot.transform.right * 0.1f);
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + targetSpeed, 0.5f);
    }
}
