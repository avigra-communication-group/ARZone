using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlaceName
{
    public string namaTempat;
    public bool state;

    public PlaceName(string name, bool st)
    {
        namaTempat = name;
        state = st;
    }
}

[System.Serializable]
public class VisitedPlaces
{
    public Dictionary<string, object> places;
}
