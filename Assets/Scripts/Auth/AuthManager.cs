using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement;


public  class AuthManager: MonoBehaviour
{

    private const uint AUTH_TIMEOUT = 30;

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
            FO.auth.StateChanged += ARAuthManager.AuthStateChanged;
            ARAuthManager.AuthStateChanged(this, null);
        }
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

        FO.auth.SignInWithCredentialAsync(FO.credential).ContinueWith(task =>
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
    
    private void ProceedToGame() {
        SceneManager.LoadScene("mainmenu");
    }
}
