// WARNING CRASH ISSUE. 
// There is crash issue for the current version of Firbase realtime database
// Follow the issue here...
// https://github.com/firebase/quickstart-unity/issues/371

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Firebase;
using Firebase.Database;
using DG.Tweening;

public class ARMapObjectController : MonoBehaviour
{

    public PlaceData placeData = null;
    private double currentPoint;
    private DatabaseReference pointReference;
    private DatabaseReference visitedPlaceReference;
    private bool pointGathered = false;
    private bool pointUpdated = false;

    void Awake()
    {

        pointReference = FO.fdb.GetReference("users").Child(FO.userId).Child("point");
        visitedPlaceReference = FO.fdb.GetReference("users").Child(FO.userId).Child("visitedPlaces");

        pointReference
            .GetValueAsync()
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Retrieving point from firebase failed " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    currentPoint = Convert.ToDouble(task.Result.Value.ToString());
                    pointGathered = true;
                }
            });
        
        visitedPlaceReference
            .GetValueAsync()
            .ContinueWith(task => {
                if(task.IsFaulted)
                {
                    Debug.Log("Retriveing visitedPlaceList from firebase failed.");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    foreach(var child in snapshot.Children)
                    {
                        FO.visitedPlace.Add(child.Key);
                    }
                }
            });
    }

    void Start()
    {
            placeData = IconObjectControllerHelperUtils.operapblePlaceData;
            IconObjectControllerHelperUtils.operapblePlaceData = null;
    }
    private void Update() 
    {
        // this is workaround because of firebase bug that doesnt work with monobehaviour nicely :(
        if(pointUpdated)
        {
            RewardPanelManager.instance.OpenPanel();
            pointUpdated = false;
        }
    }

    void AddPointToUser(string userID, double point)
    {
        if(!pointGathered)
        {
            Debug.Log("Can't update point. Origin point value not gathered yet.");
            return;
        }

        // Cannot use Task yet because of crashes issue. Use the basic setAsyncValue instead.

        // // use Transaction to securely update user point
        // pointReference.RunTransaction(data => {
        //     if(data.Value == null) data.Value = 0;
        //     data.Value = currentPoint + point;            
        //     return TransactionResult.Success(data);
        // }).ContinueWith(task => {
        //     if(task.IsFaulted)
        //     {
        //         Debug.Log(task.Exception.ToString());
        //         return;
        //     }
        //         rpm.OpenPanel();
        //         //IconObjectControllerHelperUtils.operapblePlaceData = null;
        // });

        double result = currentPoint += point;
        pointReference
            .SetValueAsync(result)
            .ContinueWith(task => {
                if(task.IsFaulted)
                {
                    Debug.Log("Point update failed. " +task.Exception);
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Point added to database successfully");
                    pointUpdated = true;
                }
            });
    }

    void AddThisPlaceToUserVisitedList()
    {
        // PlaceName thisPlace = new PlaceName(placeData.namaTempat, true);

        // visitedPlaces.placeNames.Add(thisPlace);

        // visitedPlaceReference
        //     .SetValueAsync(visitedPlaces)
        //     .ContinueWith(task =>
        //     {
        //         if (task.IsFaulted)
        //         {
        //             Debug.Log("Visited places update failed. " + task.Exception);
        //         }
        //         else if (task.IsCompleted)
        //         {
        //             Debug.Log("Visited places updated to database successfully");
        //             pointUpdated = true;
        //         }
        //     });

        visitedPlaceReference
            .Child(placeData.namaTempat)
            .SetValueAsync(true);
    }

    void OnMouseDown() {
        if(RewardPanelManager.instance == null) 
        {
            Debug.LogError("Reward panel doesn't exist.");
            return;
        }
        
        transform
            .DOScale(transform.localScale * 1.5f, 0.5f)
            .OnComplete(() => {
                transform
                    .DOScale(transform.localScale * 0f, 0.2f)
                    .OnComplete(() => {
                    AddPointToUser(FO.userId, placeData.pointFromPlace);
                    AddThisPlaceToUserVisitedList();
                });
            });     
    }
}
