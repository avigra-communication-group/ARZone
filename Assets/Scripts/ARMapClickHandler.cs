using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARMapClickHandler : MonoBehaviour
{
    private void Update() 
    {
     ClickHandler();    
    }
    
    private void ClickHandler()
    {
        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.collider.gameObject.CompareTag("ARIcon"))
                {
                    IconObjectController i = hit.collider.GetComponent<IconObjectController>();
                    if (i != null)
                    {
                        i.OnClicked();
                    }
                }
            }
        }
    }
}
