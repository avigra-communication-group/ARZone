using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GoMap;
using GoShared;

public class IconObjectController : MonoBehaviour
{
    private PlaceData placeData;
    public bool canBeClicked = false;

    public PlaceData PlaceData
    {
        get { return placeData; }
        set
        {
            placeData = value;
        }
    }

    private void Start() 
    {
        canBeClicked = !FO.visitedPlace.Contains(placeData.namaTempat);  
    }

    // private void OnEnable()
    // {
    //     canBeClicked = false;
    //     POIManager.onLocationGathered += SetButtonClickedState;
    //     //POIManager.OnLocationGathered.AddListener(this.SetButtonClickedState);
    // }

    // private void OnDisable()
    // {
    //     POIManager.onLocationGathered -= SetButtonClickedState;
    //     //POIManager.OnLocationGathered.RemoveListener(this.SetButtonClickedState);
    // }


    private void OnMouseDown() {

        if(!canBeClicked)
        {
            Debug.Log("You can not get a point from place you have already visited.");
            ModalPanelManager.instance.Choice(
                "",
                "Lokasi ini sudah Anda kunjungi sebelumnya. Kunjungi lokasi lainnya untuk mendapatkan point.",
                false,
                "",
                "",
                null,
                null,
                false
            );
            return;
        }

        AccelerometerCameraControl.SetTrackingWayWithPlayerPrefs(0);

        IconObjectControllerHelperUtils.operapblePlaceData = PlaceData;

        if(!SystemInfo.supportsGyroscope)
        {
            SceneManager.LoadScene("Demo_ARCameraACCELEROMETER");
        }
        else if (SystemInfo.supportsGyroscope)
        {
            SceneManager.LoadScene("Demo_ARCameraGYRO");
        }
        
    }
}
