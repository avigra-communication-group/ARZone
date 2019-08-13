using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class User
{
    public double point = 0;
    public VisitedPlaces visitedPlaces;

    public User()
    {
        point = 0;
        visitedPlaces = new VisitedPlaces();
    }

}
