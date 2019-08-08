using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IconObjectController : MonoBehaviour
{
    private PlaceData placeData;

    public PlaceData PlaceData
    {
        get { return placeData; }
        set
        {
            placeData = value;
        }
    }

    private void OnMouseDown() {
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
