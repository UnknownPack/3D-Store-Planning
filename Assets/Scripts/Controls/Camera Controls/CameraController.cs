using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float zoomSpeed = 1000f;
    public float rotateSpeed = 100f;
    public float tiltRange = 75f;
    
    private PlayerInput playerInput;
    private InputAction panAction, heightAction, rotateAction, zoomAction;

    private Vector2 panInput, heightInput, rotateInput;
    private float currentPitch = 0f, currentYaw = 0f;
    private float zoomInput;


    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        panAction = playerInput.actions["Pan"];
        heightAction = playerInput.actions["Height"];
        rotateAction = playerInput.actions["Rotate"];
        zoomAction = playerInput.actions["Zoom"];

        panAction.performed += ctx => panInput = ctx.ReadValue<Vector2>();
        panAction.canceled += ctx => panInput = Vector2.zero;

        heightAction.performed += ctx => heightInput = ctx.ReadValue<Vector2>();
        heightAction.canceled += ctx => heightInput = Vector2.zero;

        rotateAction.performed += ctx => rotateInput = ctx.ReadValue<Vector2>();
        rotateAction.canceled += ctx => rotateInput = Vector2.zero;

        zoomAction.performed += ctx => zoomInput = ctx.ReadValue<Vector2>().y;
        zoomAction.canceled += ctx => zoomInput = 0f;
    }

    private void Update()
    { 
        Vector3 panMovement = new Vector3(panInput.x, 0, panInput.y) * moveSpeed * Time.deltaTime;
        transform.position += panMovement;
        
        Vector3 heightMovement = new Vector3(0, heightInput.y, 0) * zoomSpeed * Time.deltaTime;
        transform.position += heightMovement; 
        
        Vector3 zoomMovement = transform.forward * (zoomInput * zoomSpeed * Time.deltaTime);
        transform.position += zoomMovement;
        
        // rotation management
        currentPitch -= rotateInput.y * rotateSpeed * Time.deltaTime;
        currentYaw -= rotateInput.x * rotateSpeed * Time.deltaTime; 
        currentPitch = Mathf.Clamp(currentPitch, -tiltRange, tiltRange);
        Camera.main.transform.localEulerAngles = new Vector3(currentPitch, currentYaw, 0);
    }
}
