using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ARCameraTargetPicker : MonoBehaviour
{

    public delegate GameObject SendObjectToCamDelegate();
    public delegate void GetObjectBackFromCamDelegate(GameObject go);

    public static event SendObjectToCamDelegate onPicked;
    public static event GetObjectBackFromCamDelegate onPickReleased;

    public Transform pickerObjectTransform;

    public Sprite picked;
    public Sprite notPicked;

    

    private GameObject activeGameObject;
    
    public GameObject ActiveGameObject
    {
        get { return activeGameObject; }
        set
        {
            activeGameObject = value;
            activeGameObject.transform.SetParent(pickerObjectTransform);
            switch (animationType)
            {
                case AnimationType.DirectorAnimation:
                    activeGameObject.transform.localPosition = Vector3.zero;
                    activeGameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    break;
                case AnimationType.VideoAnimation:
                    activeGameObject.transform.localPosition = new Vector3(0,0,-4f);
                    activeGameObject.transform.localRotation = Quaternion.Euler(new Vector3(90,0,0));
                    break;
                case AnimationType.ModelAnimation:
                    activeGameObject.transform.localPosition = Offset.offsetPosition;
                    activeGameObject.transform.localRotation = Quaternion.Euler(Offset.offsetRotation);
                    activeGameObject.transform.localScale = Offset.offsetScale;
                    break;
                default:
                    break;
            }     
        }
    }

    

    public Button pickObjectButton;
    [HideInInspector]
    public static bool togglePick = false;
    public static AnimationType animationType;
    public static TargetOffset Offset;
    private Vector3 lastTargetTransform;
    private Transform lastTargetTransformParent;

    private void Start() 
    {
        pickObjectButton.onClick.AddListener(TogglePickObject);    
    }

    public void TogglePickObject()
    {
        togglePick = ! togglePick;
        if(togglePick)
        {
            if(onPicked != null)
            {
                ActiveGameObject = onPicked.Invoke();
                pickObjectButton.GetComponent<Image>().sprite = picked;
            } 
        }
        else
        {
            if(onPickReleased != null)
            {
                onPickReleased.Invoke(ActiveGameObject);
                activeGameObject = null;
                pickObjectButton.GetComponent<Image>().sprite = notPicked;
            }   
        }
    }
    
}
