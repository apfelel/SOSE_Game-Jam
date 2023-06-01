using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject visualRootOffset;

    [SerializeField] private AudioSource windAudioSource;
    private PlayerAnimController animController;
    [SerializeField] private GameObject targetCam;
    private GameObject CamTargetOffset;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private LayerMask moveAllignLayer;
    [SerializeField] private GameObject vCam;
    [SerializeField] private float speed;
    [SerializeField] private GameObject visualRoot;
    [SerializeField] private CinemachineFreeLook freelookCam;
    [SerializeField] private float maxHeight;
    [SerializeField] private float sprintSpeed = 2;
    [SerializeField] private float maxStamina, staminaRegen, staminaRegenDelay;
    private float curStamina, staminaRegenDelayTimer;
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
        CamTargetOffset = targetCam.transform.parent.gameObject;
        animController = GetComponent<PlayerAnimController>();
        curStamina = maxStamina;
        vCam.SetActive(false);
        GameManager.LVLFinished += LVLFinished;
        GameManager.LVLStart += UnlockInput;
        UIManager.OnUnPause += UnlockInput;
        UIManager.OnPause += OnPause;
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

    private void OnPause()
    {
        windAudioSource.volume = 0;
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
        UIManager.OnPause -= OnPause;
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
        windAudioSource.volume = (rb.velocity.magnitude / (2 * speed)) - 0.2f;

        UIManager.Instance.RefreshStamina(curStamina / maxStamina);
        if(sprintAction.IsPressed())
        {
            staminaRegenDelayTimer = 0;
        }
        else
        {
            staminaRegenDelayTimer += Time.deltaTime;
        }

        if(staminaRegenDelayTimer > staminaRegenDelay)
        {
            curStamina += staminaRegen * Time.deltaTime;
            curStamina = Mathf.Clamp(curStamina, 0, maxStamina);
        }
    }
    private void LateUpdate()
    {
        CamTargetOffset.transform.localPosition = Vector3.Lerp(CamTargetOffset.transform.localPosition, cam.transform.right, Time.deltaTime * 30);

    }
    private void Move(Vector2 moveDir)
    {
        var vertical = verticalMoveAction.ReadValue<float>();
        if (sprintAction.IsPressed() && curStamina > 0 && moveDir != Vector2.zero)
        {
            sprintMod = sprintSpeed;
            curStamina -= Time.deltaTime;
        }
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
            || Physics.Raycast(transform.position + (visualRoot.transform.right * 0.2f), targetSpeed.normalized, out hit, rayDistance, moveAllignLayer)
            || Physics.Raycast(transform.position + (visualRoot.transform.right * -0.2f), targetSpeed.normalized, out hit, rayDistance, moveAllignLayer)
            || Physics.Raycast(transform.position + (visualRoot.transform.up * 0.2f), targetSpeed.normalized, out hit, rayDistance, moveAllignLayer)
            || Physics.Raycast(transform.position + (visualRoot.transform.up * -0.2f), targetSpeed.normalized, out hit, rayDistance, moveAllignLayer))
        {
            var projected = Vector3.ProjectOnPlane(targetSpeed, hit.normal);
            projected.y = Mathf.Clamp(projected.y, 0, 10000);
            targetSpeed = (targetSpeed.normalized + projected.normalized * 0.15f).normalized * targetMag;
        }

        if (Physics.Raycast(transform.position, visualRoot.transform.right, out hit, 2, moveAllignLayer) 
            || Physics.Raycast(transform.position, -visualRoot.transform.right, out hit, 2, moveAllignLayer))
        {
            var dir = (transform.position - hit.transform.position).normalized;
            dir.y = 0;
            targetSpeed = (targetSpeed.normalized - dir * 0.1f).normalized * targetMag;
        }

        if(transform.position.y > maxHeight && targetSpeed.y > 0)
        {
            var forward = targetSpeed;
            forward.y = 0;
            targetSpeed = (targetSpeed.normalized + (forward.normalized * (transform.position.y - maxHeight))).normalized * targetMag;
        }
        if (animController.Cleaning)
        {
            targetSpeed = Vector3.zero;
        }
        rb.velocity = Vector3.Lerp(rb.velocity, targetSpeed, Time.deltaTime * 10);

        freelookCam.m_Lens.FieldOfView = Mathf.Lerp(freelookCam.m_Lens.FieldOfView, 40 + (rb.velocity.magnitude * 2), Time.deltaTime);

        var test = particle.emission;
        test.rateOverTime = rb.velocity.magnitude - (speed + 1);
        visualRootOffset.transform.localPosition = Vector3.Lerp(visualRootOffset.transform.localPosition, new Vector3(0, -1 + Mathf.Clamp01(rb.velocity.magnitude + 0.1f)) / 3, Time.deltaTime / 3);
        targetCam.transform.localPosition = Vector3.Lerp(targetCam.transform.localPosition, new Vector3(0, 0.65f - (rb.velocity.magnitude / 40),0), Time.deltaTime * 5f);
    }
    private void AllignVisualRoot()
    {
        Vector3 targetForward = rb.velocity;

        if (Mathf.Abs(Vector3.Angle(targetForward, visualRoot.transform.forward)) < 5) return;

        if (targetForward.magnitude < 0.1f) targetForward = Vector3.zero;
        else
        {
            float dot = Vector3.Dot(targetForward.normalized, visualRoot.transform.forward);
            if (dot < 0.7f)
            {
                targetForward = targetForward.normalized + (visualRoot.transform.right * 0.3f);
            }
            targetForward = (targetForward + (Vector3.ProjectOnPlane(rb.velocity, visualRoot.transform.up))).normalized;
        }
       


        if(animController.Cleaning)
            visualRoot.transform.forward = Vector3.Slerp(
                visualRoot.transform.forward,
                new Vector3(visualRoot.transform.forward.x, 0, visualRoot.transform.forward.z),
                Time.deltaTime * 10);
        else
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
