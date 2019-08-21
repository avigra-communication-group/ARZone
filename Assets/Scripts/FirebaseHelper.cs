using System.Collections;
using System.Collections.Generic;
using System;
using Firebase;
using UnityEngine;
using Firebase.Database;
using Firebase.Unity.Editor;

public class FirebaseHelper : MonoBehaviour
{
    public static DatabaseReference UserRef { get; set; }
    public static DatabaseReference PointRef { get; set; }
    public static DatabaseReference VisitedLocRef { get; set; }
    public static DatabaseReference GalleryUrlsRef { get; set; }
    public static DatabaseReference RootReference { get; set; }
    private const string FIREBASE_URL = "https://ar-zone.firebaseio.com/";
    public static float elapsedTime;
    public static readonly float TIMEOUT = 12f;

    public static void Init() 
    {

        FO.userId = PlayerPrefs.GetString(UserPrefType.PLAYER_ID);
        FO.app = FirebaseApp.DefaultInstance;
        FO.app.SetEditorDatabaseUrl(FIREBASE_URL);
        FO.fdb = FirebaseDatabase.DefaultInstance;
        
        RootReference = FO.fdb.RootReference;
        UserRef = FO.fdb.GetReference("users");
        PointRef = FO.fdb.GetReference("users").Child(FO.userId).Child("point");
        VisitedLocRef = FO.fdb.GetReference("users").Child(FO.userId).Child("visitedPlaces");
        GalleryUrlsRef = FO.fdb.GetReference("gallery");
    }

    public static IEnumerator CheckIfUserIsRegistered(string uid, Action<bool> isRegistered)
    {
        elapsedTime = 0;

        if(FO.userId == "")
        {
            yield break;
        }

        var task = UserRef.Child(FO.userId).GetValueAsync();
        yield return new WaitWhile(() => IsTask(task.IsCompleted));

        if(task.IsFaulted || task.IsCanceled)
        {
            Debug.Log("Network error "+task.Exception);
            yield break;
        }

        var result = task.Result;
        
        if(result == null || result.Value == null)
        {
           Debug.Log("User is not registered. Creating new User...");
           isRegistered.Invoke(false);
            yield  break;
        }
        else
        {
            Debug.Log("User is registered.");
            isRegistered.Invoke(true);
            yield return null;
        }

        yield return null;
    }

    public static IEnumerator CreateNewUser(string uid, Action registrationSucceeded)
    {
        elapsedTime = 0;

        var task = UserRef.Child(uid).Child("point").SetValueAsync(0);
        yield return new WaitUntil(() => IsTask(task.IsCompleted));

        if(task.IsFaulted || task.IsCanceled)
        {
            Debug.Log("Creating new user failed or intterupted. Check your network connection." +task.Exception);
            yield break;
        }

        //If point successfully added, then add the visited place placeholder
        elapsedTime = 0;
        var nextTask = UserRef.Child(uid).Child("visitedPlaces").Child("t1").SetValueAsync(0);
        yield return new WaitUntil(() => IsTask(nextTask.IsCompleted));

        if (nextTask.IsFaulted || nextTask.IsCanceled)
        {
            Debug.Log("Creating new user failed or intterupted. Check your network connection." + nextTask.Exception);
            yield break;
        }

        Debug.Log("New user created.");
        registrationSucceeded.Invoke();

        yield return null;
    }

    public static IEnumerator GetUserPoint(string uid, Action<double> point)
    {
        Debug.Log("Coroutine Get User Point Called.");
        elapsedTime = 0;
        
        var task = PointRef.GetValueAsync();
        yield return new WaitUntil(() => IsTask(task.IsCompleted));

        if (task.IsFaulted || task.IsCanceled)
        {
            Debug.Log("Network error " + task.Exception);
            yield break;
        }

        var result = task.Result;

        if (result == null || result.Value == null)
        {
            Debug.Log("Error : "+task.Exception);
            yield break;
        }
        else
        {
            Debug.Log("Point gathered");
            double p = Convert.ToDouble(result.Value.ToString());
            FO.userPoint = p;
            point.Invoke(p);
            yield return null;
        }

        yield return null;
    }

    public static IEnumerator AddUserPoint(string uid, double pointAdded, Action onSuccess)
    {
        elapsedTime = 0;
        double currentPoint = 0;
        
        yield return FirebaseHelper.GetUserPoint(FO.userId, (point) => {
            currentPoint = point;
        });

        double pointResult = currentPoint + pointAdded;

        var task = PointRef.SetValueAsync(pointResult);
        yield return new WaitUntil(() => IsTask(task.IsCompleted));

        if (task.IsFaulted || task.IsCanceled)
        {
            Debug.Log("Error when updating point." + task.Exception);
            yield break;
        }

        Debug.Log("Point successfully added");
        onSuccess.Invoke();

        yield return null;
    }

    public static IEnumerator GetUserVisitedPlaces(string uid, Action<List<string>> visitedPlaces)
    {
        elapsedTime = 0;
        List<string> vp = new List<string>();

        var task = VisitedLocRef.GetValueAsync();
        yield return new WaitUntil(() => IsTask(task.IsCompleted));

        if (task.IsFaulted || task.IsCanceled)
        {
            Debug.Log("Network error " + task.Exception);
            yield break;
        }

        var result = task.Result;

        if (result == null || result.Value == null)
        {
            Debug.Log("No visited places yet");
            yield break;
        }
        else
        {
            foreach(var p in result.Children)
            {
                vp.Add(p.Key);
                yield return null;
            }
            Debug.Log("Visited place gathered.");
            visitedPlaces.Invoke(new List<string>(vp));
            yield return null;
        }
        yield return null;
    }

    public static IEnumerator GetGalleryUrls(Action<List<string>> galleryUrls)
    {
        //TASK IS BUGGY WHEN REFERENCE HAS NO VALUE!! DON'T USE THIS FOR THE MOMENT!!!!

        elapsedTime = 0;
        List<string> urls = new List<string>();
        
         Debug.Log("Get Gallery Urls try getvalue async");

         var task = GalleryUrlsRef.GetValueAsync();
         yield return new WaitUntil(() => IsTask(task.IsCompleted));

         Debug.Log("Get Gallery urls task completed");

        if (task.IsFaulted || task.IsCanceled)
        {
            Debug.Log("Network error " + task.Exception);
            yield break;
        }

        var result = task.Result;

        if (result == null)
        {
            Debug.Log("No urls yet");
            yield break;
        }
        else
        {
            foreach (var p in result.Children)
            {
                urls.Add(p.Value.ToString());
                yield return null;
            }
            Debug.Log("Gallery urls gathered.");
            galleryUrls.Invoke(new List<string>(urls));
            yield return null;
        }
        Debug.Log("Get gallery urls called.");
        yield return null;
    }

    public static void GetGalleryImagesFromDB()
    {
        Debug.Log("Retrieving gallery image urls database");
        List<string> galleryUrls = new List<string>();

        GalleryUrlsRef
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

    public static bool IsTask(bool value)
    {
        elapsedTime += Time.deltaTime;

        if (value)
        {
            return false;
        }
        else
        {
            if (elapsedTime > TIMEOUT)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

}
