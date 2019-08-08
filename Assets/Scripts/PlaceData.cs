using System.Collections;
using UnityEngine;

[System.Serializable]
public class PlaceData 
{
    [SerializeField]
    [Header("Nama tempat harus unik!")]
    public string namaTempat;
    [SerializeField]
    public double lat;
    [SerializeField]
    public double lon;
    [SerializeField]
    public PlaceCategory kategori;
    [SerializeField]
    public double pointFromPlace = 0;
    [SerializeField]
    public int objectModelID;
}