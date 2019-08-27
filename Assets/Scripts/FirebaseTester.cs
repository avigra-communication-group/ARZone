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
            FirebaseHelper.CheckIfUserIsRegistered(FO.userId, (isRegistered) => {
                Debug.Log("User status is "+isRegistered);
            });
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            FirebaseHelper.GetUserPoint(FO.userId, (p) => Debug.Log("Point is " +p));
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            double add = 10;
            Debug.Log("added user point by 10 ");
            FirebaseHelper.AddUserPoint(FO.userId, add, ()=> {
                Debug.Log("Point added successfully from tester.");
            });
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Check user visitedplaces");

            FirebaseHelper.GetUserVisitedPlaces(FO.userId, (visitedPlaces) =>
            {
                foreach (var i in visitedPlaces)
                {
                    Debug.Log(i);
                }
            });
        }
        #endif
    }
}
