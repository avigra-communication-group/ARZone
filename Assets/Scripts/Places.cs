using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Place", menuName="Place", order = 51)]
public class Places : ScriptableObject {
    public string namaDaerah;
    public List<PlaceData> placeList = new List<PlaceData>();
}