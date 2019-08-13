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
    public delegate void ValueChangedListener(object sender, ValueChangedEventArgs args);

    public static event OnRegisterDelegate onRegisterDelegate;
    public static event OnPointValueChanged onPointValueChanged;

    private const string DB_BASEURL = "https://ar-zone.firebaseio.com/";

    public static DataSnapshot userSnapshot;
    public User currentUser;

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
        #if UNITY_EDITOR
        PlayerPrefs.SetString(UserPrefType.PLAYER_ID, "eid");
        #endif
        
        FO.userId = PlayerPrefs.GetString(UserPrefType.PLAYER_ID);
        
        FO.app = FirebaseApp.DefaultInstance;
        FO.fdb = FirebaseDatabase.DefaultInstance;
        CheckUserAvalability();
    }

    private void CheckUserAvalability() 
    {

        if(onRegisterDelegate != null)
        {
            onRegisterDelegate.Invoke(false);
        }

        if (FO.userId == "")
        {
            Debug.Log("uid is empty. User is not registered");
            FO.userIsRegistered = false;
        }
        else
        {
            Debug.Log("User ID found = " +FO.userId);
        }

        FO.fdb
            .GetReference("users")
            .Child(FO.userId)
            .GetValueAsync()
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // error handling here
                    Debug.Log("Data retriving process failed. Task terminated.");
                    return;
                }
                else if (task.IsCompleted)
                {
                    // data retrieved. Now check if user exist or no
                    Debug.Log("Data retrieved. User Exist = " +task.Result.Exists);
                    userSnapshot = task.Result;
                    FO.userIsRegistered = userSnapshot.Exists;
                    if (FO.userIsRegistered)
                    {
                        if(onRegisterDelegate != null) onRegisterDelegate.Invoke(true);

                        Debug.Log("User registered.");
                        GetUser((userGathered) => {
                            Debug.Log("User gathered!");
                        });
                        return;
                    }
                    else
                    {
                        if(onRegisterDelegate != null) onRegisterDelegate.Invoke(false);

                        Debug.Log("User not registered.");
                        Debug.Log("Creating new user with id :" +FO.userId);
                        CreateNewUser(FO.userId);
                    }
                }
            });
    }

    public void CreateNewUser(string id)
    {
        Debug.Log("Create a new user instance...");

        // currentUser = new User();

        // FO.currentUser = currentUser;

        // string json = JsonUtility.ToJson(currentUser);

        // FO.fdb.GetReference("users").Child(id).SetRawJsonValueAsync(json);

        FO.fdb
            .GetReference("users")
            .Child(id)
            .Child("point")
            .SetValueAsync(0);

        FO.fdb
            .GetReference("users")
            .Child(id)
            .Child("visitedPlaces")
            .Child("t1")
            .SetValueAsync(true);

        if (onRegisterDelegate != null) onRegisterDelegate.Invoke(true);
        if (onPointValueChanged != null) onPointValueChanged.Invoke(0);

        Debug.Log(FO.userId +" user created");
    }

    public void AddListenerToDB()
    {
        FO.fdb
            .GetReference("users")
            .Child(FO.userId)
            .ValueChanged += ValueChangedHandler;
    }

    public void ValueChangedHandler(object sender, ValueChangedEventArgs args)
    {
        if(args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        DataSnapshot snapshot = args.Snapshot;
        string json = snapshot.GetRawJsonValue();
        currentUser = JsonUtility.FromJson<User>(json);
        FO.currentUser = currentUser;
    }

    public void GetUser(Action<User> userGathered) 
    {
        FO.fdb
            .GetReference("users")
            .Child(FO.userId)
            .GetValueAsync()
            .ContinueWith(task => {
                if(task.IsFaulted)
                {

                }
                else if (task.IsCompleted)
                {
                    DataSnapshot userSnapshot = task.Result;
                    string json = userSnapshot.GetRawJsonValue();
                    currentUser= JsonUtility.FromJson<User>(json);
                    FO.currentUser = currentUser;
                    if(userGathered != null) userGathered.Invoke(currentUser);
                }
            });
        // FirebaseDatabase.DefaultInstance
        //     .GetReference("users")
        //     .Child(FO.userId)
        //     .Child("point")
        //     .GetValueAsync()
        //     .ContinueWith(task =>
        //     {
        //         if (task.IsFaulted)
        //         {
        //             Debug.Log("Data retriving process failed. Task terminated.");
        //             return;
        //         }
        //         else if (task.IsCompleted)
        //         {
        //             Debug.Log("Point retrieved with result "+task.Result);
        //             DataSnapshot snapshot = task.Result;
        //             FO.currentUser.point = Convert.ToDouble(snapshot.Value.ToString());
        //             if(onPointValueChanged != null) onPointValueChanged.Invoke(FO.currentUser.point);
        //             pointGathered(true);
                    
        //         }
        //     }
        // );
    }

    public void AddUserPoint(string id, double addedPoint)
    {
        currentUser.point += addedPoint;

        string json = JsonUtility.ToJson(currentUser);

        FirebaseDatabase.DefaultInstance.GetReference("users").Child(id).SetRawJsonValueAsync(json);
    }

}
