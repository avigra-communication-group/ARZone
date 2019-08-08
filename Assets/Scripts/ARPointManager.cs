using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;

public class ARPointManager : MonoBehaviour
{

    public delegate void OnRegisterDelegate(bool state);
    public delegate void OnPointValueChanged(double value);

    public static event OnRegisterDelegate onRegisterDelegate;
    public static event OnPointValueChanged onPointValueChanged;

    private const string DB_BASEURL = "https://ar-zone.firebaseio.com/";

    DataSnapshot userPoint;

    // Methods start here ==================
    // ====================================
    // ====================================

    private void Update() {
        // for debug
        if(Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log(PlayerPrefs.GetString(UserPrefType.PLAYER_ID));
        }
    }

    private void Start()
    {
        CheckUserAvalability();
    }

    private void CheckUserAvalability() 
    {

        onRegisterDelegate.Invoke(false);

        if (!PlayerPrefs.HasKey(UserPrefType.PLAYER_ID))
        {
            FO.userIsRegistered = false;
            #if UNITY_EDITOR
            PlayerPrefs.SetString(UserPrefType.PLAYER_ID, "eid");
            #endif
        }

        FO.playerId = PlayerPrefs.GetString(UserPrefType.PLAYER_ID);

        FirebaseDatabase.DefaultInstance
            .GetReference("users")
            .Child(FO.playerId)
            .GetValueAsync()
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Data retriving process failed. Task terminated.");
                    return;
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Data retrieved. User Exist = " +task.Result.Exists);
                    DataSnapshot user = task.Result;
                    FO.userIsRegistered = user.Exists;

                    if (FO.userIsRegistered)
                    {
                        if(onRegisterDelegate != null) onRegisterDelegate.Invoke(true);
                        Debug.Log("User registered.");
                        return;
                    }
                    else
                    {
                        Debug.Log("Creating new user...");
                        AddUser();
                    }
                }
            });
    }

    public void AddUser() 
    {
        FirebaseDatabase.DefaultInstance
        .GetReference("users")
        .Child(FO.playerId)
        .Child("point")
        .SetValueAsync(0);


        if (onRegisterDelegate != null) onRegisterDelegate.Invoke(true);
        if (onPointValueChanged != null)  onPointValueChanged.Invoke(0);
        
        Debug.Log("User created with id : "+FO.playerId);
    }

    public static void GetPoint(Action<bool> pointGathered) 
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("users")
            .Child(FO.playerId)
            .Child("point")
            .GetValueAsync()
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Data retriving process failed. Task terminated.");
                    return;
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Point retrieved with result "+task.Result);
                    DataSnapshot snapshot = task.Result;
                    FO.point = Convert.ToDouble(snapshot.Value.ToString());
                    if(onPointValueChanged != null) onPointValueChanged.Invoke(FO.point);
                    pointGathered(true);
                    
                }
            }
        );
    }

    public static void SetPoint(double addedPoint)
    {
        double result = FO.point + addedPoint;

        FirebaseDatabase.DefaultInstance
            .GetReference("users")
            .Child(FO.playerId)
            .Child("point")
            .SetValueAsync(result);

        GetPoint(null);
    }

}
