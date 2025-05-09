using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float zoomSpeed  = 1000f;
    [SerializeField] private float rotateSpeed  = 100f;
    [SerializeField] private float tiltRange  = 75f;
    [SerializeField] private float incrament  = 1f;
    [SerializeField] private LayerMask floorlayer;
    [SerializeField] public Material highlightMaterial;
    [SerializeField] public Material highlight_LockMaterial;

    private PlayerInput playerInput;
    UiManager uiManager;
    
    #region Camera-related fields
    
        private InputAction panAction, heightAction, rotateAction, zoomAction;
        private Vector2 panInput, heightInput, rotateInput;
        private float currentPitch = 0f, currentYaw = 0f;
        private float zoomInput;
        
    #endregion 
    #region Input fields 
        private InputAction objectSpecificAction;
        private GameObject currentSelectedObject;
        private MeshRenderer currentSelectedObject_MeshRenderer;
        private Vector3 currentMousePositionToWorld;
        private Material copy;
        private bool bObjectManipulation = false, bIncramentalChange = false, draggingGameObject = false, placementMode  = false;
    #endregion
 

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        uiManager = GetComponent<UiManager>();

        #region Camera-related initalizations
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
        #endregion  
        
        #region Input initalizations 
            objectSpecificAction = playerInput.actions["ObjectSpecific"];
     
            objectSpecificAction.performed += ctx => 
            {
                bObjectManipulation = !bObjectManipulation;
            };

        #endregion
    }

    private void Update()
    {
        ManageCameraMovement();
        ManageScreenInput();
        ManageSelectedObject(); 
    }

    void ManageCameraMovement()
    {
        Vector3 panDirection = new Vector3(panInput.x, 0f, panInput.y);
        Vector3 rotatedPan = Quaternion.Euler(0f, currentYaw, 0f) * panDirection;
        transform.position += rotatedPan * moveSpeed * Time.deltaTime;

        
        Vector3 heightMovement = new Vector3(0, heightInput.y, 0) * moveSpeed * Time.deltaTime;
        transform.position += heightMovement; 
        
        Vector3 zoomMovement = transform.forward * (zoomInput * zoomSpeed * Time.deltaTime);
        transform.position += zoomMovement;
        
        // if user is in object manipulation mode, they can use rotate keys to rotate object instead of camera
        if(!bObjectManipulation)
        {
            currentPitch -= rotateInput.y * rotateSpeed * Time.deltaTime;
            currentYaw -= rotateInput.x * rotateSpeed * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch, -tiltRange, tiltRange);
            Camera.main.transform.localEulerAngles = new Vector3(currentPitch, currentYaw, 0);
        }
    }

    void ManageScreenInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit floorHit, Mathf.Infinity, floorlayer))
        {
            currentMousePositionToWorld = floorHit.point;
            Debug.DrawRay(ray.origin, ray.direction * floorHit.distance, Color.green);
        }

        // terminate function early if in different mode
        if(placementMode)
            return;
        
        // select object
        if (Input.GetMouseButtonDown(0))
        {
            Ray clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(clickRay, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Movable"))
                {
                    currentSelectedObject = hit.collider.gameObject;
                    uiManager.SetSelectableObject(hit.collider.gameObject);
                    draggingGameObject = true;
                    currentSelectedObject_MeshRenderer = currentSelectedObject.GetComponent<MeshRenderer>();
                    copy = currentSelectedObject_MeshRenderer.material;
                    currentSelectedObject_MeshRenderer.material = highlightMaterial;
                }
                else if(!bObjectManipulation && !hit.collider.CompareTag("Movable"))
                {
                    if (currentSelectedObject_MeshRenderer != null)
                        currentSelectedObject_MeshRenderer.material = copy;
                    uiManager.SetSelectableObject(null);
                    currentSelectedObject = null;
                    draggingGameObject = false;
                    bObjectManipulation = false;
                }
            }
        }

        // drag object if left mouse held down
        if (Input.GetMouseButton(0) && draggingGameObject && currentSelectedObject != null)
            currentSelectedObject.transform.position = new Vector3(currentMousePositionToWorld.x, currentSelectedObject.transform.localScale.y / 2, currentMousePositionToWorld.z);

        // On release of left mouse, stop dragging
        if (Input.GetMouseButtonUp(0))
        {
            draggingGameObject = false;
            if (currentSelectedObject_MeshRenderer != null)
                currentSelectedObject_MeshRenderer.material = copy;
        }

        // right click to deselect
        if (Input.GetMouseButtonDown(1))
        {
            if (currentSelectedObject_MeshRenderer != null)
                currentSelectedObject_MeshRenderer.material = copy;
            currentSelectedObject = null;
            draggingGameObject = false;
            bObjectManipulation = false;
        }
    }


    void ManageSelectedObject()
    {
        if (currentSelectedObject == null)
            return;

        if(bObjectManipulation)
            currentSelectedObject.transform.Rotate(Vector3.up, rotateInput.x * incrament, Space.World);
        
        if (Input.GetKeyDown(KeyCode.R))
            currentSelectedObject.transform.rotation = Quaternion.identity;

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
            Instantiate(currentSelectedObject, 
                new Vector3(currentMousePositionToWorld.x, currentSelectedObject.transform.localScale.y / 2, currentMousePositionToWorld.z), 
                currentSelectedObject.transform.rotation);
        
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            uiManager.SetSelectableObject(null);
            Destroy(currentSelectedObject);
            currentSelectedObject = null;
            draggingGameObject = false;
        }
    }

    #region Public Methods
        public GameObject CurrentSelectedObject { get { return currentSelectedObject; } }
        public LayerMask GetFloorMask(){return floorlayer;}
        public void SetSelectedObject(GameObject obj){currentSelectedObject = obj;}
        public void SetPlacementMode(bool mode){placementMode = mode;}
    #endregion


}


