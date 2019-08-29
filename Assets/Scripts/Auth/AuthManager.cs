using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Auth;
using UnityEngine.SceneManagement;


public  class AuthManager: MonoBehaviour
{

    private const uint AUTH_TIMEOUT = 12 * 1000;

    void Start() 
    {
        LoadPlayerPref();
        InitFirebase();
        AuthUIManager.instance.signUpButton.onClick.AddListener(this.SignInPlayer);
        //AuthUIManager.instance.verificationButton.onClick.AddListener(SignInWithVerificationCode);
    }

    private void LoadPlayerPref() {
        AuthUIManager.instance.signUpPhoneInput.text = PlayerPrefs.GetString(UserPrefType.PLAYER_ID);
    }

    private void InitFirebase() {
        if(FO.auth == null) {
            FO.auth = FirebaseAuth.DefaultInstance;
            FO.auth.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);
        }
    }

    private void OnDestroy()
    {
        FO.auth.StateChanged -= AuthStateChanged;
    }

    // Sign in Player
    private void SignInPlayer() 
    {
        string typedPhoneNumber = AuthUIManager.instance.signUpPhoneInput.text;
        string phoneNumber = "+62" + typedPhoneNumber.TrimStart('0');
        AuthUIManager.instance.SetMessage("Melakukan pendaftaran... .");
        FO.phoneAuthProvider = PhoneAuthProvider.GetInstance(FO.auth);

        FO.phoneAuthProvider.VerifyPhoneNumber(phoneNumber, AUTH_TIMEOUT, null,
        verificationCompleted: (credential) => {
            // Auto-sms-retrieval or instant validation has succeeded (Android only).
            // There is no need to input the verification code.
            // `credential` can be used instead of calling GetCredential().
            PlayerPrefs.SetString(UserPrefType.PLAYER_ID, typedPhoneNumber.ToString());
            FO.credential = credential;
            FO.auth
                .SignInWithCredentialAsync(FO.credential)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.Log("Proses pendaftaran dibatalkan.");
                        AuthUIManager.instance.SetMessage("Proses pendaftaran dibatalkan");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.Log("Proses pendaftaran belum berhasil. " + task.Exception);
                        AuthUIManager.instance.SetMessage("Proses pendaftaran belum berhasil. " + task.Exception);
                    }
                    if (task.IsCompleted)
                    {
                        Debug.Log("Pendaftaran berhasil.");
                        AuthUIManager.instance.SetMessage("Pendaftaran berhasil.");
                        FO.user = task.Result;
                        ProceedToGame();
                    }
                });
            AuthUIManager.instance.SetMessage("Verifikasi berhasil. ");
            ProceedToGame();
        },
        verificationFailed: (error) => {
            // The verification code was not sent.
            // `error` contains a human readable explanation of the problem.
            AuthUIManager.instance.SetMessage("Verifikasi belum berhasil. Cobalah beberapa saat lagi." +error);
        },
        codeSent: (id, token) => {
            // Verification code was successfully sent via SMS.
            // `id` contains the verification id that will need to passed in with
            // the code from the user when calling GetCredential().
            // `token` can be used if the user requests the code be sent again, to
            // tie the two requests together.
            PlayerPrefs.SetString(UserPrefType.VERIFICATION_ID, id);
            AuthUIManager.instance.SetMessage("Id verifikasi: " +id);
            AuthUIManager.instance.DisplayPanel(AuthPanelType.Verification, true);
        },
        codeAutoRetrievalTimeOut: (id) => {
            // Called when the auto-sms-retrieval has timed out, based on the given
            // timeout parameter.
            // `id` contains the verification id of the request that timed out.
            //AuthUIManager.instance.DisplayPanel(AuthPanelType.Verification, true);
        });
    }

    public void SignInWithVerificationCode() {
        string verId = PlayerPrefs.GetString(UserPrefType.VERIFICATION_ID);
        string responseCode = AuthUIManager.instance.verificationResponseCodeInput.text;

        if(verId == "") {
            Debug.Log("Sign in canceled, no verification id.");
            AuthUIManager.instance.SetMessage("Sign in canceled, no verification id.");
            return;
        } else if(responseCode == "") {
            Debug.Log("Masukkan response code Anda.");
            AuthUIManager.instance.SetMessage("Masukkan response code Anda.");
            return;
        }
        
        FO.credential = FO.phoneAuthProvider.GetCredential(verId, responseCode);

        FO.auth
            .SignInWithCredentialAsync(FO.credential)
            .ContinueWithOnMainThread(task =>
            {
                AuthUIManager.instance.SetMessage("Mencoba verifikasi....");
                if (task.IsFaulted)
                {
                    Debug.Log("Sign in with credential async encountered an error: " + task.Exception);
                    AuthUIManager.instance.SetMessage("Sign in with credential async encountered an error: " + task.Exception);
                    SceneManager.LoadScene("auth");
                    return;
                }
                FO.user = task.Result;
                Debug.Log("User signed in successfully.");
                AuthUIManager.instance.SetMessage("Welcome user " + FO.user.PhoneNumber);
                ProceedToGame();
            });
    }

    public void ResendVerificationCode()
    {
        // not implemented
    }
    
    private void ProceedToGame() {
        SceneManager.LoadScene("mainmenu");
    }

    public void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (FO.auth.CurrentUser != FO.user)
        {
            FO.isSignedIn = FO.user != FO.auth.CurrentUser && FO.auth.CurrentUser != null;

            if (!FO.isSignedIn && FO.user != null)
            {
                Debug.Log("Signed out " + FO.user.UserId);
                // if signed out tell user to login or sign-in
                // AuthUIManager.instance.authContainer.SetActive(true);
                // AuthUIManager.instance.DisplayPanel(AuthPanelType.SignUp, true);
                // GoToScene("auth");
                // SceneManager.LoadScene("auth");
            }

            FO.user = FO.auth.CurrentUser;

            if (FO.isSignedIn)
            {
                // GoToScene("mainmenu");
                SceneManager.LoadScene("mainmenu");
            }
        }
    }
}
