using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoMap;
using GoShared;

public class POIManager : MonoBehaviour
{
    public Places places;
    public GOMap goMap;

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
        
        foreach(PlaceData place in places.placeList) {
            GameObject temp = Instantiate(place.prefab);
            goMap.dropPin(place.lat, place.lon, temp);
            Debug.Log("Dropping pin "+place.namaTempat);
        }
    }

}
