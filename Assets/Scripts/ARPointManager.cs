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
    public List<string> locationVisited;
    public List<string> galleryUrls;

    // Methods start here ==================
    // ====================================
    // ====================================

    private void Update() {
        // for debug
        if(Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log(PlayerPrefs.GetString(UserPrefType.PLAYER_ID));
        }
        if(Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("point : " +FO.userPoint);
            foreach(string p in FO.visitedPlace)
            {
                Debug.Log(p);
            }
        }
    }

    private void Awake()
    {
        #if UNITY_EDITOR
        PlayerPrefs.SetString(UserPrefType.PLAYER_ID, "eid");
        #endif
        
        FO.userId = PlayerPrefs.GetString(UserPrefType.PLAYER_ID);
        FO.app = FirebaseApp.DefaultInstance;
        FO.fdb = FirebaseDatabase.DefaultInstance;
        CheckUserAvalability();
        GetVisitedMapFromDB();
        GetGalleryImagesFromDB();
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
                    ModalPanelManager.instance.Choice(
                        "Network error",
                        "Maaf nampaknya ada kendala pada jaringan Anda. Cobalah beberapa saat lagi.",
                        false,
                        "",
                        "",
                        null,
                        null,
                        false
                    );
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
                        FO.userPoint =  Convert.ToDouble(userSnapshot.Child("point").Value);
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

        FO.fdb
            .GetReference("users")
            .Child(id)
            .Child("point")
            .SetValueAsync(0)
            .ContinueWith(task => {
                if(task.IsFaulted)
                {
                    //error handling
                } 
                else if(task.IsCompleted)
                {
                    FO.userPoint = 0;
                }
            });

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
        // string json = snapshot.GetRawJsonValue();
        // currentUser = JsonUtility.FromJson<User>(json);
        // FO.userPoint = currentUser.point;
        
        // update user point
        FO.userPoint =  Convert.ToDouble(snapshot.Child("point").Value);
        Debug.Log("Point updated to " +FO.userPoint);

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
                    //currentUser= JsonUtility.FromJson<User>(json);
                    if(userGathered != null) userGathered.Invoke(currentUser);
                }
            });
    }

    public void AddUserPoint(string id, double addedPoint)
    {
        currentUser.point += addedPoint;

        string json = JsonUtility.ToJson(currentUser);

        FirebaseDatabase.DefaultInstance.GetReference("users").Child(id).SetRawJsonValueAsync(json);
    }

    public void GetVisitedMapFromDB()
    {
        Debug.Log("Retrieving visited place database");
        FO.fdb
            .GetReference("users")
            .Child(FO.userId)
            .Child("visitedPlaces")
            .GetValueAsync()
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Failed to retrieve visited places for user " + FO.userId + ". " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Successfully retrieved visited places data for user " + FO.userId);
                    DataSnapshot snapshot = task.Result;
                    //FO.visitedPlace.Clear();

                    foreach (var child in snapshot.Children)
                    {
                        Debug.Log("adding " +child.Key+ " to locationVisited.");
                        locationVisited.Add(child.Key);
                    }

                    FO.visitedPlace = new List<string>(locationVisited);

                    Debug.Log("Location Gathered");

                }
            });
    }

    public void GetGalleryImagesFromDB()
    {

        Debug.Log("Retrieving gallery image urls database");
        FO.fdb
            .GetReference("gallery")
            .GetValueAsync()
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Failed to retrieve visited places for user " + FO.userId + ". " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Successfully retrieved gallery urls data");
                    DataSnapshot snapshot = task.Result;
                    //FO.visitedPlace.Clear();

                    foreach (var child in snapshot.Children)
                    {
                        Debug.Log("adding " + child.Value + " to gallery urls.");
                        galleryUrls.Add(child.Value.ToString());
                    }

                    FO.galleryImages = new List<string>(galleryUrls);

                    Debug.Log("gallery urls data gathered");
                }
            });
    }
}
