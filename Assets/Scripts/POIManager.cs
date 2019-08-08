using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoMap;
using GoShared;

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

    Coordinates coordinateGPS;

    private void Awake()
    {
        goMap.locationManager.onOriginSet.AddListener((Coordinates) => { GeneratePlaces(Coordinates); });
    }

    // private void Start() 
    // {
    //     if(places != null) {
    //         GeneratePlaces();
    //     }
    // }

    private void GeneratePlaces(Coordinates currentLocation) 
    {
        for (int i = 0; i < places.placeList.Count; i++)
        {
            PlaceData currentPlace = places.placeList[i];
            GameObject temp = Instantiate(objectDatabase[currentPlace.objectModelID].iconObject);

            IconObjectController iconController = temp.GetComponent<IconObjectController>();

            if (iconController != null)
            {
                iconController.PlaceData = currentPlace;
            }

            goMap.dropPin(currentPlace.lat, currentPlace.lon, temp);
            
            Debug.Log("Dropping pin " + currentPlace.namaTempat);
        }
    }

}
