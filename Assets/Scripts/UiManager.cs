using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UiManager : MonoBehaviour
{
    public List<GameObject> Prefabs = new List<GameObject>();
    public VisualTreeAsset btnTemplate;
    public VisualTreeAsset inspectorTemplate;
    private GameObject selectedObject = null, lastInspectedObject = null, potentialGameObject = null;
    private MeshRenderer potentialGameObject_MeshRenderer = null;
    private Material potentialGameObject_Material = null;
    private bool inPlacementMode = false;
    private UIDocument uiDocument;
    private CameraController cameraController;
    private ListView listView;
    private VisualElement root, inspectorContainer;

    void Start()
    {
        cameraController = GetComponent<CameraController>();
        uiDocument = GetComponent<UIDocument>();

        if (uiDocument == null)
        {
            Debug.LogError("No UIDocument found");
            return;
        }

        root = uiDocument.rootVisualElement;
        inspectorContainer = root.Q<VisualElement>("inspector");

        #region ListView initalisation
            listView = root.Q<ListView>("prefabList");
            listView.makeItem = () => btnTemplate.Instantiate();
            listView.bindItem = (element, i) =>
            {
                var button = element.Q<Button>("btn");
                button.text = Prefabs[i].gameObject.name;
                button.clicked += () =>
                {
                    if (cameraController == null)
                    {
                        Debug.LogError("cameraController is null");
                        return;
                    }
                    Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f);
                    Ray ray = Camera.main.ScreenPointToRay(screenCenter);

                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, cameraController.GetFloorMask()))
                    {
                        potentialGameObject = Instantiate(Prefabs[i], hit.point, Quaternion.identity);
                        potentialGameObject_MeshRenderer = potentialGameObject.GetComponent<MeshRenderer>();
                        potentialGameObject_Material = potentialGameObject_MeshRenderer.material;
                        potentialGameObject_MeshRenderer.material = cameraController.highlightMaterial;
                        inPlacementMode = true;
                        cameraController.SetPlacementMode(true); 
                    } 
                };
            };

            listView.itemsSource = Prefabs;
            listView.fixedItemHeight = 70f;
            listView.RefreshItems();
            inspectorContainer.style.display = DisplayStyle.None;
        #endregion
 
    }

    void Update()
    {
        if (inPlacementMode)
        {
            Ray clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(clickRay, out RaycastHit hit, Mathf.Infinity, cameraController.GetFloorMask()))
            {
                Vector3 heighOffset = new Vector3(0, potentialGameObject.transform.localScale.y / 2, 0);
                potentialGameObject.transform.position = hit.point + heighOffset;
                if (Input.GetMouseButtonDown(0))
                {
                    potentialGameObject = null;
                    potentialGameObject_MeshRenderer.material = potentialGameObject_Material;
                    cameraController.SetPlacementMode(false);
                    inPlacementMode = false;
                }
            }
        }
        if(selectedObject!=null)
            ManageBasicInspector(selectedObject); 
    }

    void ManageBasicInspector(GameObject selected)
    {
        if (lastInspectedObject == selected) return;
        
        lastInspectedObject = selected; 
        inspectorContainer.Clear();
        inspectorContainer.Add(inspectorTemplate.CloneTree());
        var inspectorRoot = root.Q<VisualElement>("ins");
        string cleanedName = selected.name.Replace("(Clone)", "").Trim();
        inspectorRoot.Q<Label>("objName").text = cleanedName;
        for (int i = 1; i < 4; i++)
        {
            VisualElement floatContainer = inspectorRoot.Children().ElementAt(i);
            var floatFields = floatContainer.Query<FloatField>().ToList();
            
            Vector3 pos = Vector3.zero;
            switch (i)
            {
                case 1: pos = selected.transform.position; break;
                case 2: pos = selected.transform.rotation.eulerAngles; break;
                case 3: pos = selected.transform.localScale; break;
            }
 
            floatFields[0].value = pos.x;
            floatFields[1].value = pos.y;
            floatFields[2].value = pos.z;

            int transformMode = i; 
            for (int j = 0; j < 3; j++)
            {
                RegisterFieldCallback(floatFields[j], selected, transformMode, floatFields);
            }
        }
    }

    #region Helper Functions
        
    Vector3 GetVectorFromFields(List<FloatField> fields)
    {
        return new Vector3(fields[0].value, fields[1].value, fields[2].value);
    }

    void ApplyTransform(GameObject obj, int mode, Vector3 value)
    {
        switch (mode)
        {
            case 1:
                obj.transform.position = value;
                break;
            case 2:
                obj.transform.rotation = Quaternion.Euler(value);
                break;
            case 3:
                obj.transform.localScale = value;
                break;
        }
    }
    
    void RegisterFieldCallback(FloatField field, GameObject targetObject, int mode, List<FloatField> fields)
    {
        field.RegisterValueChangedCallback(evt =>
        {
            Vector3 newValue = GetVectorFromFields(fields);
            ApplyTransform(targetObject, mode, newValue);
        });
    }
    

        #endregion
 
    public void SetSelectableObject(GameObject obj)
    {
        selectedObject = obj;
        inspectorContainer.style.display = (selectedObject != null) ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
