// Script ini berlaku sebagai bank buat UI Elements

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Firebase.Auth;

public enum AuthPanelType {
    SignUp,
    Verification,
    LogIn
}

public class AuthUIManager : MonoBehaviour
{
    public static AuthUIManager instance = null;

    public GameObject authContainer;
    public GameObject signUpPanel;
    public GameObject verificationPanel;
    public GameObject logInPanel;

    public InputField signUpPhoneInput;
    public InputField verificationResponseCodeInput;

    public Button signUpButton;
    public Button signOutButton;
    public Button verificationButton;
    public Button resendVerificationButton;
    public Button logInButton;

    public Text debugText;

    private void Awake() 
    {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(this.gameObject);
        }
    }

    private void Start() 
    {
        Init();    
    }

    public void Init()
    {
        debugText.text = "";
        signUpPhoneInput.text = PlayerPrefs.GetString(UserPrefType.PLAYER_ID) ?? "";
    }

    public void DisplayPanel(AuthPanelType panelType, bool state) {
        switch (panelType)
        {
            case AuthPanelType.SignUp:
                signUpPanel.SetActive(state);
                verificationPanel.SetActive(!state);
                logInPanel.SetActive(!state);
                break;
            case AuthPanelType.Verification:
                verificationPanel.SetActive(state);
                signUpPanel.SetActive(!state);
                logInPanel.SetActive(!state);
                break;
            case AuthPanelType.LogIn:
                logInPanel.SetActive(state);
                signUpPanel.SetActive(!state);
                verificationPanel.SetActive(!state);
                break;
            default:
            break;
        }
    }

    public void SetMessage(string text)
    {
        CancelInvoke();
        string currentText = debugText.text;
        debugText.text = currentText + "\n" +text;
        Invoke("ClearMessage", 4f);
    }

    private void ClearMessage(){
        Color currentTextColor = debugText.color;

        debugText.DOFade(0f, 2f).OnComplete(() => {
            debugText.text = "";
            debugText.color = currentTextColor;
        });
    }
}
