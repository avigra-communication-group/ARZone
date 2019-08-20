// Tulis semua method dan aktivitas yang ingin di preload dalam script ini

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using UnityEngine.UI;

public class PreloadActivity : MonoBehaviour
{

    public Text messageText;
    private const uint AUTH_TIMEOUT = 30;

    private void Start() {
        StartCoroutine(InitFirebase());
    }

    private void GoToScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    // Init firebase
    IEnumerator InitFirebase()
    {
        messageText.text = "";
        messageText.text = "Initializing authentication server... .";

        FO.auth = FirebaseAuth.DefaultInstance;
        FO.auth.StateChanged +=  ARAuthManager.AuthStateChanged;
        ARAuthManager.AuthStateChanged(this, null);

        yield return new WaitForSeconds(2f);

        StartCoroutine(SignInPlayer());
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
            messageText.text = "Verifikasi berhasil.";
            GoToScene("mainmenu");
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
            messageText.text  = "Menunggu verifikasi.";
            GoToScene("auth");
        });
    }
}
