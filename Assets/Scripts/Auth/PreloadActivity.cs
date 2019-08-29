// Tulis semua method dan aktivitas yang ingin di preload dalam script ini

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.UI;

public class PreloadActivity : MonoBehaviour
{

    public Text messageText;
    private const uint AUTH_TIMEOUT = 12 * 1000;

    private void Start() {
        InitFirebase();
    }

    private void GoToScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    // Init firebase
    private void  InitFirebase()
    {
        messageText.text = "";
        messageText.text = "Memulai proses authentikasi... .";

        FO.auth = FirebaseAuth.DefaultInstance;
        FO.auth.StateChanged +=  AuthStateChanged;
        AuthStateChanged(this, null);

        StartCoroutine(SignInPlayer());
    }

    private void OnDestroy() 
    {
        FO.auth.StateChanged -= ARAuthManager.AuthStateChanged;
    }

    // Sign in Player
    IEnumerator SignInPlayer()
    {
        string phoneNumber =   "+62" +PlayerPrefs.GetString(UserPrefType.PLAYER_ID).TrimStart('0');
        messageText.text ="Melakukan pendaftaran... .";
        FO.phoneAuthProvider = PhoneAuthProvider.GetInstance(FO.auth);

        yield return new WaitForSeconds(2f);

        FO.phoneAuthProvider.VerifyPhoneNumber(phoneNumber, AUTH_TIMEOUT, FO.forceResendingToken,
        verificationCompleted: (credential) =>
        {
            // Auto-sms-retrieval or instant validation has succeeded (Android only).
            // There is no need to input the verification code.
            // `credential` can be used instead of calling GetCredential().
            FO.credential = credential;
            FO.auth
                .SignInWithCredentialAsync(FO.credential)
                .ContinueWithOnMainThread(task => {
                    if(task.IsCanceled)
                    {
                        Debug.Log("Proses pendaftaran dibatalkan.");
                        messageText.text = "Proses pendaftaran dibatalkan.";
                        return;
                    }
                    if(task.IsFaulted)
                    {
                        Debug.Log("Proses pendaftaran belum berhasil. " + task.Exception);
                        messageText.text = "Proses pendaftaran belum berhasil.";
                    }
                    if(task.IsCompleted)
                    {
                        Debug.Log("Pendaftaran berhasil.");
                        FO.user = task.Result;
                        messageText.text = "Pendaftaran berhasil.";
                        GoToScene("mainmenu");
                    }
                });
        },
        verificationFailed: (error) =>
        {
            // The verification code was not sent.
            // `error` contains a human readable explanation of the problem.
            messageText.text = "Verifikasi otomatis belum berhasil. " +error;
            GoToScene("auth");
        },
        codeSent: (id, token) =>
        {
            // Verification code was successfully sent via SMS.
            // `id` contains the verification id that will need to passed in with
            // the code from the user when calling GetCredential().
            // `token` can be used if the user requests the code be sent again, to
            // tie the two requests together.
            PlayerPrefs.SetString(UserPrefType.VERIFICATION_ID, id.ToString());
        },
        codeAutoRetrievalTimeOut: (id) =>
        {
            // Called when the auto-sms-retrieval has timed out, based on the given
            // timeout parameter.
            // `id` contains the verification id of the request that timed out.
            messageText.text  = "Verifikasi otomatis belum berhasil. Melakukan proses pendaftaran manual.";
            GoToScene("auth");
        });
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
                GoToScene("mainmenu");
            }
        }
    }
}
