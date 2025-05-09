using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UiManager : MonoBehaviour
{
    public List<GameObject> Prefabs = new List<GameObject>();
    public VisualTreeAsset template;
    private GameObject selectedObject = null;
    private GameObject lastInspectedObject = null;

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
        listView = root.Q<ListView>("prefabList");

        listView.makeItem = () => template.Instantiate();
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
                    GameObject newGameObject = Instantiate(Prefabs[i], hit.point, Quaternion.identity);
                    cameraController.SetSelectedObject(newGameObject);
                } 
            };
        };

        listView.itemsSource = Prefabs;
        listView.fixedItemHeight = 70f;
        listView.RefreshItems();
    }

    void Update()
    {
        if (selectedObject != null)
            ManageBasicInspector(selectedObject);
    }

    void ManageBasicInspector(GameObject selected)
    {
        if (lastInspectedObject == selected) return;
        lastInspectedObject = selected;

        Label objNameLabel = inspectorContainer.Q<Label>("objName");
        objNameLabel.text = selected.name;

        for (int i = 1; i < 4; i++)
        {
            VisualElement floatContainer = inspectorContainer.Children().ElementAt(i);
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
                floatFields[j].RegisterValueChangedCallback(evt =>
                {
                    Vector3 newVal = GetVectorFromFields(floatFields);
                    ApplyTransform(selected, transformMode, newVal);
                });
            }
        }
    }

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

    public void SetSelectableObject(GameObject obj)
    {
        selectedObject = obj;
    }
}
