using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseTester : MonoBehaviour
{

    private void Start() {
        FirebaseHelper.Init();
    }

    private void Update() 
    {
        #if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("Check user registered");
            StartCoroutine(FirebaseHelper.CheckIfUserIsRegistered(FO.userId, (isRegistered) => {
                Debug.Log("User status is "+isRegistered);
            }));
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Check user point");
            StartCoroutine(FirebaseHelper.GetUserPoint(FO.userId, (point) =>
            {
                Debug.Log("User point is " + point);
            }));
            
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Check user visitedplaces");
            StartCoroutine(FirebaseHelper.GetUserVisitedPlaces(FO.userId, (visitedPlaces) => {
                foreach(var i in visitedPlaces)
                {
                    Debug.Log(i);
                }
            }));
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            double add = 10;
            Debug.Log("added user point by 10 ");
            StartCoroutine(FirebaseHelper.AddUserPoint(FO.userId, add, () => {
                Debug.Log("Point added.");
                StartCoroutine(FirebaseHelper.GetUserPoint(FO.userId, (point) =>
                {
                    Debug.Log("User point is " + point);
                }));
            }));
            
        }    
    }
    #endif
}
