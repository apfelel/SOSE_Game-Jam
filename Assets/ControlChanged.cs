using UnityEngine;

public class ControlChanged : MonoBehaviour
{
    private UnityEngine.InputSystem.PlayerInput playerInput;
    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        GameManager.Instance.Keyboard = playerInput.currentControlScheme != "Gamepad";
    }
}
