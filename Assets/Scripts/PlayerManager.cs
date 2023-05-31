using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject targetCam;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private LayerMask moveAllignLayer;
    [SerializeField] private GameObject vCam;
    [SerializeField] private float speed;
    [SerializeField] private GameObject visualRoot;
    [SerializeField] private CinemachineFreeLook freelookCam;
    [SerializeField] private float maxHeight;
    [SerializeField] private float sprintSpeed = 2;
    private float sprintMod = 1.5f;
    PlayerInput input;
    InputAction moveAction;
    InputAction verticalMoveAction;
    InputAction interactAction;
    InputAction pauseAction;
    InputAction lookAction;
    InputAction sprintAction;
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
        UIManager.OnSettingsClosed += RefreshSensitivity;
        cam = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        playerPickup = GetComponentInChildren<PlayerPickup>();

        input = new();
        moveAction = input.Player.Move;

        sprintAction = input.Player.Sprint;

        interactAction = input.Player.Interact;
        interactAction.performed += Interact;

        pauseAction = input.Player.Pause;
        pauseAction.performed += PauseGame;

        lookAction = input.Player.Look;

        verticalMoveAction = input.Player.VerticalMove;
    }

    private void RefreshSensitivity(float Value)
    {
        freelookCam.m_YAxis.m_MaxSpeed = 0.001f * Value;
        freelookCam.m_XAxis.m_MaxSpeed = 0.3f * Value;
    }

    private void LVLFinished()
    {
        this.enabled = false;
    }

    private void PauseGame(InputAction.CallbackContext obj)
    {
        vCam.SetActive(false);
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
        sprintAction.Enable();
        lookAction.Enable();
    }
    private void OnDisable()
    {
        GameManager.LVLFinished -= LVLFinished;
        GameManager.LVLStart -= UnlockInput;
        UIManager.OnUnPause -= UnlockInput;
        UIManager.OnSettingsClosed -= RefreshSensitivity;
        pauseAction.Disable();
        verticalMoveAction.Disable();
        moveAction.Disable();
        interactAction.Disable();
        lookAction.Disable();
        vCam.SetActive(false);
    }
    private void LockInput()
    {
        vCam.SetActive(false);
        moveAction.Disable();
        interactAction.Disable();
        sprintAction.Disable();
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
        if (sprintAction.IsPressed())
            sprintMod = sprintSpeed;
        else
            sprintMod = 1;

        Vector3 newTargetSpeed;
        if (Cam)
        {
            newTargetSpeed = (cam.transform.forward * moveDir.y + cam.transform.right * moveDir.x) * speed * sprintMod;
            newTargetSpeed.y = (vertical * 10);
        }
        else
        {
            newTargetSpeed = (cam.transform.forward * moveDir.y + cam.transform.right * moveDir.x) * speed * sprintMod;
        }

        if(sprintMod < 1.4f && rb.velocity.magnitude < 8)
        {
            targetSpeed = newTargetSpeed * Mathf.Clamp(Vector3.Dot(newTargetSpeed.normalized, newTargetSpeed.normalized) + 0.4f, 0.1f, 1f);
        }
        else
        {
            targetSpeed = (newTargetSpeed.normalized + rb.velocity.normalized).normalized * newTargetSpeed.magnitude;
        }
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
            projected.y = Mathf.Clamp(projected.y, 0, 10000);
            targetSpeed = (targetSpeed.normalized + projected.normalized * 0.25f).normalized * targetMag;
        }

        if(transform.position.y > maxHeight && targetSpeed.y > 0)
        {
            var forward = targetSpeed;
            forward.y = 0;
            targetSpeed = (targetSpeed.normalized + (forward.normalized * (transform.position.y - maxHeight))).normalized * targetMag;
        }

        rb.velocity = Vector3.Lerp(rb.velocity, targetSpeed, Time.deltaTime * 10);

        freelookCam.m_Lens.FieldOfView = Mathf.Lerp(freelookCam.m_Lens.FieldOfView, 40 + rb.velocity.magnitude, Time.deltaTime);

        var test = particle.emission;
        test.rateOverTime = rb.velocity.magnitude - 15;

        targetCam.transform.localPosition = Vector3.Lerp(targetCam.transform.localPosition, new Vector3(0, 2f - (rb.velocity.magnitude / 40),0), Time.deltaTime * 5f);
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
