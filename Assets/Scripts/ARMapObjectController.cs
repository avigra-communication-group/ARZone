using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARMapObjectController : MonoBehaviour
{
    public PlaceData placeData = null;

    void Start()
    {
            placeData = IconObjectControllerHelperUtils.operapblePlaceData;
            IconObjectControllerHelperUtils.operapblePlaceData = null;
    }
}
