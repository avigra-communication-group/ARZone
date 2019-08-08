using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRotator : MonoBehaviour
{
    [Range(0, 100)]
    public float rotationSpeed = 20f;

    private void Update()
    {
        transform.Rotate(0,rotationSpeed * Time.deltaTime, 0);
    }
}
