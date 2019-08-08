using System.Collections;
using Firebase;
using Firebase.Auth;

public static class FO
{
    public static FirebaseAuth auth;
    public static FirebaseUser user;
    public static FirebaseApp app;
    public static PhoneAuthProvider phoneAuthProvider;
    public static Credential credential;
    public static bool isSignedIn = false;

    // for user
    public static double point;
    public static string playerId;
    public static bool userIsRegistered;
}