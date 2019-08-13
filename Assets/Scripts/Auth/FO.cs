using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public static class FO
{
    public static FirebaseAuth auth;
    public static FirebaseUser user;
    public static FirebaseApp app;
    public static FirebaseDatabase fdb;
    public static PhoneAuthProvider phoneAuthProvider;
    public static Credential credential;
    public static bool isSignedIn = false;

    // for user
    public static string userId;
    public static User currentUser = null;
    public static List<string> visitedPlace;
    public static bool userIsRegistered;
}