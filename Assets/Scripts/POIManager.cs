using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GoMap;
using GoShared;
using Firebase;
using Firebase.Database;

[System.Serializable]
public class POIObjectDB {
    public int objectID;
    public GameObject iconObject;
}

public class POIManager : MonoBehaviour
{
    public Places places;
    public GOMap goMap;
    public List<POIObjectDB> objectDatabase;
    public static UnityEvent OnLocationGathered;
    public delegate void OLC();
    public static OLC onLocationGathered;

    private DataSnapshot snapshot;


    Coordinates coordinateGPS;

    private void Awake()
    {
        // goMap.locationManager.GetVisitedMapFromDB(() => {
        //     goMap.locationManager.onOriginSet.AddListener((Coordinates) => { GeneratePlaces(Coordinates); });
        // });
        
        goMap.locationManager.onOriginSet.AddListener((Coordinates) => { GeneratePlaces(Coordinates); });
    }

    private void OnDisable() 
    {
        goMap.locationManager.onOriginSet.RemoveAllListeners();
        BaseLocationManager.IsOriginSet = false;
    }

    private void GeneratePlaces(Coordinates currentLocation) 
    {
        for (int i = 0; i < places.placeList.Count; i++)
        {
            PlaceData currentPlace = places.placeList[i];
            GameObject temp = Instantiate(objectDatabase[currentPlace.objectModelID].iconObject);
            temp.name = currentPlace.namaTempat + " _ARZoneIcon";
            IconObjectController iconController = temp.GetComponent<IconObjectController>();

            if (iconController != null)
            {
                iconController.PlaceData = currentPlace;
                // if (FO.visitedPlace.Contains(currentPlace.namaTempat))
                // {
                //     iconController.canBeClicked = false;
                // }
                // else
                // {
                //     iconController.canBeClicked = true;
                // }
            }

            goMap.dropPin(currentPlace.lat, currentPlace.lon, temp);
            
            Debug.Log("Dropping pin " + currentPlace.namaTempat);
        }
    }

}
