using System.Collections;
using System.Collections.Generic;
using System;
using Firebase;
using UnityEngine;
using Firebase.Database;

public class FirebaseHelper : MonoBehaviour
{
    public static DatabaseReference UserRef { get; set; }
    public static DatabaseReference PointRef { get; set; }
    public static DatabaseReference VisitedLocRef { get; set; }
    public static DatabaseReference GalleryUrls { get; set; }
    private const string FIREBASE_URL = "https://ar-zone.firebaseio.com/";
    public static float elapsedTime;
    public static readonly float TIMEOUT = 12f;

    public static void Init() 
    {
            UserRef = FO.fdb.GetReference("users");
            PointRef = FO.fdb.GetReference("users").Child(FO.userId).Child("point");
            VisitedLocRef = FO.fdb.GetReference("users").Child(FO.userId).Child("visitedPlaces");
            GalleryUrls = FO.fdb.GetReference("gallery");
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

            // Create new user
        }
        else
        {
            Debug.Log("User is registered. "+result.Value);
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
            point.Invoke(p);
        }

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
            }
            visitedPlaces.Invoke(new List<string>(vp));
        }
        yield return null;
    }

    public static IEnumerator GetGalleryUrls(Action<List<string>> galleryUrls)
    {
        elapsedTime = 0;
        List<string> gal = new List<string>();

        var task = GalleryUrls.GetValueAsync();
        yield return new WaitUntil(() => IsTask(task.IsCompleted));

        if (task.IsFaulted || task.IsCanceled)
        {
            Debug.Log("Network error " + task.Exception);
            yield break;
        }

        var result = task.Result;

        if (result == null || result.Value == null)
        {
            Debug.Log("No gallery urls added.");
            yield break;
        }
        else
        {
            foreach (var p in result.Children)
            {
                gal.Add(p.Key);
            }
            galleryUrls.Invoke(new List<string>(gal));
        }
        yield return null;
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
